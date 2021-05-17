using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region GameManager Singleton
    private static GameManager _instance;

    public static GameManager Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        time = GameTimer;
        hexMap = GameObject.Find("HexMap").GetComponent<HexMap>();
        player1 = new Player(team1, 1, true, hexMap);
        player2 = new Player(team2, 2, false, hexMap);
        gameState = draft;
        GameObject[] f;
        f = GameObject.FindGameObjectsWithTag("Flag");
        foreach (var flag in f)
        {
            flags.Add(flag.GetComponent<Flag>());
        }
    }
    #endregion

    public  int GameTimer = 30;
    public float time;
    public int numberOfMoves = 0;

    public Player player1;
    List<Unit> team1 = new List<Unit>();
    public Player player2;
    List<Unit> team2 = new List<Unit>();

    public HexMap hexMap { get; private set; }

    GameState gameState;
    public readonly Game game = new Game();
    public readonly EndGame endGame = new EndGame();
    public readonly Draft draft = new Draft();

    public Transform CameraHolder;
    public Camera mainCamera;
    public Camera UICamera;

    [HideInInspector]
    public List<Flag> flags;
    public GameObject LevelUpParticle;

    [Header("Flags")]
    public Texture neutral;
    public Texture team1red;
    public Texture team2blue;

    [Header("Draft")]
    public Button[] heroes;
    public GameObject[] heroesDragAndDrop;
    public GameObject[] draftIcons;
    private int draftTurn = 0;
    public Image pickImage;
    public Animator draftAnimator;
    public GameObject description;
    public Text pickTeam;

    [Header("Game")]
    public Button endturn;
    public Text timeText;
    public Text gemsText;

    [Header("Hero Stats")]
    public Text heroName;
    public Image heroImage;
    public GameObject[] heroLvl;
    public Button[] abilityBtn;
    public Text[] cdAbility;
    public Text health;
    public Text damage;
    public Text range;
    public Text armor;
    public Text magicRes;
    public Animator showGUI;

    [Header("EndGame")]
    public GameObject canvasWin;
    public Text whoWin;

    public List<Button> kings;
    public List<Button> t1;
    public List<Button> t2;

    void Start()
    {
        foreach (var okvir in kings)
        {

             okvir.gameObject.transform.parent.gameObject.GetComponent<Image>().color = Color.cyan;
            
        }
        foreach (var button in t1)
        {
            button.interactable = false;
            button.gameObject.GetComponent<Image>().color = Color.grey;
        }
        foreach (var button in t2)
        {
            button.interactable = false;
            button.gameObject.GetComponent<Image>().color = Color.grey;
        }


    }


    void Update()
    {
        gameState.Execute();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (var item in hexMap.hexes)
            {
                if (!item.Walkable)
                {
                    item.SetColor(Color.black);
                }
            }
        }
    }

    public void ChangeState(GameState newState)
    {
        gameState.Exit();
        this.gameState = newState;
        gameState.Enter();
    }



    public void pickHero(int index)
    {
        if (draftTurn == 1)
        {
            foreach (var button in t1)
            {
                button.interactable = true;
                button.gameObject.GetComponent<Image>().color = Color.white;
                button.gameObject.transform.parent.gameObject.GetComponent<Image>().color = Color.cyan;
            }
        }
        if (draftTurn == 7)
        {
            foreach (var button in t2)
            {
                button.interactable = true;
                button.gameObject.GetComponent<Image>().color = Color.white;
                button.gameObject.transform.parent.gameObject.GetComponent<Image>().color = Color.cyan;
            }
        }
        
        if (draftTurn % 2 == 0)
        {
            if (draftTurn == 0)
            {
                //spawn king
                GameObject kingGO = heroesDragAndDrop[index].GetComponent<MouseDragHero>().prefabToInstantiate;
                GameObject unitGo = Instantiate(kingGO, new Vector3(0, 0, 0), Quaternion.identity);
                Unit unitKing = unitGo.GetComponent<Unit>();
                if (unitKing != null)
                {
                    player1.squad.Add(unitKing);
                }
                unitKing.tileCol = player1.startHex.Col;
                unitKing.tileRow = player1.startHex.Row;
                unitKing.team = player1.team;

                // anim pick
                draftIcons[draftTurn].GetComponentInChildren<IconsController>().sprite = heroes[index].GetComponent<Image>().sprite;

                heroes[index].interactable = false;
                heroes[index].gameObject.transform.parent.gameObject.GetComponent<Image>().color = Color.blue;
                draftTurn++;
            }
            else
            {
                GameObject unitGo = Instantiate(heroesDragAndDrop[index], new Vector3(0, 0, 0), Quaternion.identity);
                unitGo.GetComponent<MouseDragHero>().uiCamera = UICamera;
                unitGo.transform.SetParent(UICamera.gameObject.GetComponentInChildren<Canvas>().gameObject.transform);

                unitGo.transform.localRotation = Quaternion.Euler(0, 0, 0);

                // anim pick
                draftIcons[draftTurn].GetComponentInChildren<IconsController>().sprite = heroes[index].GetComponent<Image>().sprite;

                player1.dragdrop.Add(unitGo);
                heroes[index].interactable = false;
                heroes[index].gameObject.transform.parent.gameObject.GetComponent<Image>().color = Color.blue;
                draftTurn++;
            }

            foreach (var king in kings)
            {
                if (king.interactable == true)
                {
                    king.gameObject.transform.parent.gameObject.GetComponent<Image>().color = new Color32(255, 189, 51, 255);
                }
            }

            foreach (var t in t1)
            {
                if (t.interactable == true)
                {
                    t.gameObject.transform.parent.gameObject.GetComponent<Image>().color = new Color32(255, 189, 51, 255);
                }
            }

            foreach (var t in t2)
            {
                if (t.interactable == true)
                {
                    t.gameObject.transform.parent.gameObject.GetComponent<Image>().color = new Color32(255, 189, 51, 255);
                }
            }
            pickTeam.text = "Team 2 pick";
        }
        else
        {
            if (draftTurn == 1)
            {
                //spawn king
                GameObject kingGO = heroesDragAndDrop[index].GetComponent<MouseDragHero>().prefabToInstantiate;
                GameObject unitGo = GameObject.Instantiate(kingGO, new Vector3(0, 0, 0), Quaternion.Euler(0,180,0)) ;
                Unit unitKing = unitGo.GetComponent<Unit>();
                if (unitKing != null)
                {
                    player2.squad.Add(unitKing);
                }
                unitKing.tileCol = player2.startHex.Col;
                unitKing.tileRow = player2.startHex.Row;
                unitKing.team = player2.team;

                // anim pick
                draftIcons[draftTurn].GetComponentInChildren<IconsController>().sprite = heroes[index].GetComponent<Image>().sprite;

                heroes[index].interactable = false;
                heroes[index].gameObject.transform.parent.gameObject.GetComponent<Image>().color = Color.red;
                draftTurn++;
            }
            else
            {
                GameObject unitGo = Instantiate(heroesDragAndDrop[index], new Vector3(0, 0, 0), Quaternion.identity);
                unitGo.GetComponent<MouseDragHero>().uiCamera = UICamera;
                unitGo.transform.SetParent(UICamera.gameObject.GetComponentInChildren<Canvas>().gameObject.transform);
                unitGo.transform.localRotation = Quaternion.Euler(0, 0, 0);

                // anim pick
                draftIcons[draftTurn].GetComponentInChildren<IconsController>().sprite = heroes[index].GetComponent<Image>().sprite;

                player2.dragdrop.Add(unitGo);
                heroes[index].interactable = false;
                heroes[index].gameObject.transform.parent.gameObject.GetComponent<Image>().color = Color.red;
                draftTurn++;
            }

            foreach (var king in kings)
            {
                if (king.interactable == true)
                {
                    king.gameObject.transform.parent.gameObject.GetComponent<Image>().color = Color.cyan;
                }
            }

            foreach (var t in t1)
            {
                if (t.interactable == true)
                {
                    t.gameObject.transform.parent.gameObject.GetComponent<Image>().color = Color.cyan;
                }
            }

            foreach (var t in t2)
            {
                if (t.interactable == true)
                {
                    t.gameObject.transform.parent.gameObject.GetComponent<Image>().color = Color.cyan;
                }
            }
            pickTeam.text = "Team 1 pick";
        }

        if (draftTurn == 12)
        {
            float posX = -675;
            foreach (var unitTeam1 in player1.dragdrop)
            {
                RectTransform rectTransform = unitTeam1.GetComponent<RectTransform>();
                if (rectTransform != null)
                 {
                    rectTransform.GetComponent<RectTransform>().anchorMin = new Vector2(1, 0);
                    rectTransform.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0);
                    rectTransform.GetComponent<RectTransform>().pivot = new Vector2(1, 1);
                    rectTransform.anchoredPosition3D = new Vector3(posX, 25,5);
                 }

                unitTeam1.GetComponent<MouseDragHero>().startPos = unitTeam1.transform.localPosition;
                posX += 150;
                unitTeam1.SetActive(true);
            }
            posX = -675;
            foreach (var unitTeam2 in player2.dragdrop)
            {
                RectTransform rectTransform = unitTeam2.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.GetComponent<RectTransform>().anchorMin = new Vector2(1, 0);
                    rectTransform.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0);
                    rectTransform.GetComponent<RectTransform>().pivot = new Vector2(1, 1);
                    rectTransform.anchoredPosition3D = new Vector3(posX, 25, 5);
                }
                unitTeam2.GetComponent<MouseDragHero>().startPos = unitTeam2.transform.localPosition;
                posX += 150;
                unitTeam2.SetActive(false);
            }
            pickTeam.text = "Let's Play a Game";
            StartCoroutine(coroutineA());
        }
    }



    IEnumerator coroutineA()
    {
        yield return new WaitForSeconds(2.0f);
        SettingsControll.Instance.audioSource[0].Stop();
        SettingsControll.Instance.audioSource[1].Play();
        SettingsControll.Instance.draftToGame();
        GameObject.FindWithTag("Arrow").SetActive(false);
        ChangeState(game);
    }

    public void endTurn()
    {
        time = 0;
    }
    public void updateUnitStats(Unit unit)
    {
        if (player1.selectedUnit != null && player1.selectedUnit == unit)
        {
            health.text = ((int)unit.Stats.Health).ToString();
            damage.text = ((int)unit.Stats.Damage).ToString();
            range.text = unit.Stats.AttackRange.ToString();
            armor.text = unit.Stats.Armor.ToString();
            magicRes.text = unit.Stats.MagicResist.ToString() + "%";
            if (unit.getLevel() != 1)
            {
                if (unit.getLevel() != 2)
                {
                    if (unit.getLevel() != 3)
                    {
                        heroLvl[0].SetActive(true);
                        heroLvl[1].SetActive(true);
                        heroLvl[2].SetActive(true);
                    }
                }
                else
                {
                    heroLvl[0].SetActive(true);
                    heroLvl[1].SetActive(true);
                    heroLvl[2].SetActive(false);
                }
            }
            else
            {
                heroLvl[0].SetActive(true);
                heroLvl[1].SetActive(false);
                heroLvl[2].SetActive(false);
            }

            if (unit.ability1 != null)
            {
                cdAbility[0].text = unit.ability1.getCoolDown().ToString();
                if (unit.ability1.getCoolDown() != 0 || unit.ability1.isPassive)
                {
                    abilityBtn[0].interactable = false;
                    if (unit.ability1.isPassive)
                    {
                        cdAbility[0].text = "";
                    }
                }
                else
                {
                    abilityBtn[0].interactable = true;
                }
            }

            if (unit.ability2 != null)
            {
                cdAbility[1].text = unit.ability2.getCoolDown().ToString();
                if (unit.ability2.getCoolDown() != 0 || unit.ability2.isPassive)
                {
                    abilityBtn[1].interactable = false;
                    if (unit.ability2.isPassive)
                    {
                        cdAbility[1].text = "";
                    }
                }
                else
                {
                    abilityBtn[1].interactable = true;
                }
            }

            if (unit.ability3 != null)
            {
                cdAbility[2].text = unit.ability3.getCoolDown().ToString();
                if (unit.ability3.getCoolDown() != 0 || unit.ability3.isPassive)
                {
                    abilityBtn[2].interactable = false;
                    if (unit.ability3.isPassive)
                    {
                        cdAbility[2].text = "";
                    }
                }
                else
                {
                    abilityBtn[2].interactable = true;
                }
            }


        }
        else if(player2.selectedUnit != null && player2.selectedUnit == unit)
        {
            health.text = ((int)unit.Stats.Health).ToString();
            damage.text = ((int)unit.Stats.Damage).ToString();
            range.text = unit.Stats.AttackRange.ToString();
            armor.text = unit.Stats.Armor.ToString();
            magicRes.text = unit.Stats.MagicResist.ToString();
            if (unit.getLevel() != 1)
            {
                if (unit.getLevel() != 2)
                {
                    if (unit.getLevel() != 3)
                    {
                        heroLvl[0].SetActive(true);
                        heroLvl[1].SetActive(true);
                        heroLvl[2].SetActive(true);
                    }
                }
                else
                {
                    heroLvl[0].SetActive(true);
                    heroLvl[1].SetActive(true);
                    heroLvl[2].SetActive(false);
                }
            }
            else
            {
                heroLvl[0].SetActive(true);
                heroLvl[1].SetActive(false);
                heroLvl[2].SetActive(false);
            }

            if (unit.ability1 != null)
            {
                cdAbility[0].text = unit.ability1.getCoolDown().ToString();
                if (unit.ability1.getCoolDown() != 0 || unit.ability1.isPassive)
                {
                    abilityBtn[0].interactable = false;
                    if (unit.ability1.isPassive)
                    {
                        cdAbility[0].text = "";
                    }
                }
                else
                {
                    abilityBtn[0].interactable = true;
                }
            }

            if (unit.ability2 != null)
            {
                cdAbility[1].text = unit.ability2.getCoolDown().ToString();
                if (unit.ability2.getCoolDown() != 0 || unit.ability2.isPassive)
                {
                    abilityBtn[1].interactable = false;
                    if (unit.ability2.isPassive)
                    {
                        cdAbility[1].text = "";
                    }
                }
                else
                {
                    abilityBtn[1].interactable = true;
                }
            }

            if (unit.ability3 != null)
            {
                cdAbility[2].text = unit.ability3.getCoolDown().ToString();
                if (unit.ability3.getCoolDown() != 0 || unit.ability3.isPassive)
                {
                    abilityBtn[2].interactable = false;
                    if (unit.ability3.isPassive)
                    {
                        cdAbility[2].text = "";
                    }
                }
                else
                {
                    abilityBtn[2].interactable = true;
                }
            }
        }
       
    }
    public void UpdateGUI(Player player, Unit selectedUnit)
    {
        heroName.text = selectedUnit.heroName;
        heroImage.sprite = selectedUnit.heroImage;
        health.text = ((int)selectedUnit.Stats.Health).ToString();
        damage.text = ((int)selectedUnit.Stats.Damage).ToString();
        range.text = selectedUnit.Stats.AttackRange.ToString();
        armor.text = selectedUnit.Stats.Armor.ToString();
        magicRes.text = selectedUnit.Stats.MagicResist.ToString() + "%";
        if (selectedUnit.getLevel() != 1)
        {
            if (selectedUnit.getLevel() != 2)
            {
                if (selectedUnit.getLevel() != 3)
                {
                    heroLvl[0].SetActive(true);
                    heroLvl[1].SetActive(true);
                    heroLvl[2].SetActive(true);
                }
            }
            else
            {
                heroLvl[0].SetActive(true);
                heroLvl[1].SetActive(true);
                heroLvl[2].SetActive(false);
            }
        }
        else
        {
            heroLvl[0].SetActive(true);
            heroLvl[1].SetActive(false);
            heroLvl[2].SetActive(false);
        }
        if (player.team == selectedUnit.team)
        {

            if (selectedUnit.ability1 != null)
            {
                abilityBtn[0].gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(-130,0);
                abilityBtn[0].gameObject.SetActive(true);
                abilityBtn[0].image.sprite = selectedUnit.ability1Image;
                cdAbility[0].text = selectedUnit.ability1.getCoolDown().ToString();
                if (selectedUnit.ability1.getCoolDown() != 0 || selectedUnit.ability1.isPassive)
                {
                    abilityBtn[0].interactable = false;
                    if (selectedUnit.ability1.isPassive)
                    {
                        cdAbility[0].text = "";
                    }
                }
                else
                {
                    abilityBtn[0].interactable = true;
                }
            }
            else
            {
                abilityBtn[0].gameObject.SetActive(false);
            }
            if (selectedUnit.ability2 != null)
            {
                
                abilityBtn[1].gameObject.SetActive(true);
                abilityBtn[1].image.sprite = selectedUnit.ability2Image;
                cdAbility[1].text = selectedUnit.ability2.getCoolDown().ToString();
                if (selectedUnit.ability2.getCoolDown() != 0 || selectedUnit.ability2.isPassive)
                {
                    abilityBtn[1].interactable = false;
                    if (selectedUnit.ability2.isPassive)
                    {
                        cdAbility[1].text = "";
                    }
                }
                else
                {
                    abilityBtn[1].interactable = true;
                }
            }
            else
            {
                abilityBtn[0].gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0);
                abilityBtn[1].gameObject.SetActive(false);
            }
            if (selectedUnit.ability3 != null)
            {
                abilityBtn[2].gameObject.SetActive(true);
                abilityBtn[2].image.sprite = selectedUnit.ability3Image;
                cdAbility[2].text = selectedUnit.ability3.getCoolDown().ToString();
                if (selectedUnit.ability3.getCoolDown() != 0 || selectedUnit.ability3.isPassive)
                {
                    abilityBtn[2].interactable = false;
                    if (selectedUnit.ability3.isPassive)
                    {
                        cdAbility[2].text = "";
                    }
                }
                else
                {
                    abilityBtn[2].interactable = true;
                }
            }
            else
            {
                abilityBtn[2].gameObject.SetActive(false);
            }
        }
        else
        {
            if (selectedUnit.ability1 != null)
            {
                abilityBtn[0].gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(-130, 0);
                abilityBtn[0].gameObject.SetActive(true);
                abilityBtn[0].image.sprite = selectedUnit.ability1Image;
                abilityBtn[0].interactable = false;
                cdAbility[0].text = "";
            }
            else
            {
                abilityBtn[0].gameObject.SetActive(false);
            }
            if (selectedUnit.ability2 != null)
            {
                abilityBtn[1].gameObject.SetActive(true);
                abilityBtn[1].image.sprite = selectedUnit.ability2Image;
                abilityBtn[1].interactable = false;
                cdAbility[1].text = "";

            }
            else
            {
                abilityBtn[0].gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0);
                abilityBtn[1].gameObject.SetActive(false);
            }
            if (selectedUnit.ability3 != null)
            {
                abilityBtn[2].gameObject.SetActive(true);
                abilityBtn[2].image.sprite = selectedUnit.ability3Image;
                abilityBtn[2].interactable = false;
                cdAbility[2].text = "";
            }
            else {
                abilityBtn[2].gameObject.SetActive(false); 
            }
        }
    }

    public void BackToMenu()
    {
        if (Screen.fullScreen)
        {
            PlayerPrefs.SetInt("checkbox", 1);
        }
        else
        {
            PlayerPrefs.SetInt("checkbox", 0);
        }

        PlayerPrefs.SetInt("widthRes", Screen.currentResolution.width);
        PlayerPrefs.SetInt("heightRes", Screen.currentResolution.height);
        PlayerPrefs.SetFloat("volume", SettingsControll.Instance.audioSlider.value);
        PlayerPrefs.SetFloat("volumeEff", SettingsControll.Instance.audioSliderEff.value);
        SceneManager.LoadScene("Menu");
    }
    public void PlayAgain()
    {
        if (Screen.fullScreen)
        {
            PlayerPrefs.SetInt("checkbox", 1);
        }
        else
        {
            PlayerPrefs.SetInt("checkbox", 0);
        }

        PlayerPrefs.SetInt("widthRes", Screen.currentResolution.width);
        PlayerPrefs.SetInt("heightRes", Screen.currentResolution.height);
        PlayerPrefs.SetFloat("volume", SettingsControll.Instance.audioSlider.value);
        PlayerPrefs.SetFloat("volumeEff", SettingsControll.Instance.audioSliderEff.value);


        SceneManager.LoadScene("Game");
    }
    public void ExitGame()
    {
        if (Screen.fullScreen)
        {
            PlayerPrefs.SetInt("checkbox", 1);
        }
        else
        {
            PlayerPrefs.SetInt("checkbox", 0);
        }

        PlayerPrefs.SetInt("widthRes", Screen.currentResolution.width);
        PlayerPrefs.SetInt("heightRes", Screen.currentResolution.height);
        PlayerPrefs.SetFloat("volume", SettingsControll.Instance.audioSlider.value);
        PlayerPrefs.SetFloat("volumeEff", SettingsControll.Instance.audioSliderEff.value);
        Application.Quit();
    }

}
