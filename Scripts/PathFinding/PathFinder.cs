using PathFinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PathFinder
{
    public static List<Hex> FindPath_AStar(HexMap grid, Hex start, Hex end)
    {

        grid.clearIndicators();
        grid.ResetGrid();

        foreach (var tile in grid.hexes)
        {
          
            tile.Cost = int.MaxValue;
        }

        start.Cost = 0;
        Comparison<Hex> heuristicComparison = (lhs, rhs) =>
        {
            float lhsCost = lhs.Cost + GetEuclideanHeuristicCost(lhs, end);
            float rhsCost = rhs.Cost + GetEuclideanHeuristicCost(rhs, end);

            return lhsCost.CompareTo(rhsCost);
        };
        MinHeap<Hex> frontier = new MinHeap<Hex>(heuristicComparison);
        frontier.Add(start);

        HashSet<Hex> visited = new HashSet<Hex>();
        visited.Add(start);

        start.PrevTile = null;

        while (frontier.Count > 0)
        {
            Hex current = frontier.Remove();

            if (current == end)
            {
                break;
            }

            foreach (var neighbor in grid.GetNeighbors(current))
            {
              
                if (neighbor.Walkable)
                {
                   
                    int newNeighborCost = current.Cost + neighbor.Weight;
                    if (newNeighborCost < neighbor.Cost)
                    {
                        neighbor.Cost = newNeighborCost;
                        neighbor.PrevTile = current;
                      
                    }

                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        frontier.Add(neighbor);

                        
                    }
                }
            }
        }
        List<Hex> path = BacktrackToPath(end,grid);
        return path;
    }

    public static bool InRange(HexMap grid, Hex start, Hex end, int range)
    {
        foreach (var tile in grid.hexes)
        {

            tile.Cost = int.MaxValue;
        }

        start.Cost = 0;
        Comparison<Hex> heuristicComparison = (lhs, rhs) =>
        {
            float lhsCost = lhs.Cost + GetEuclideanHeuristicCost(lhs, end);
            float rhsCost = rhs.Cost + GetEuclideanHeuristicCost(rhs, end);

            return lhsCost.CompareTo(rhsCost);
        };
        MinHeap<Hex> frontier = new MinHeap<Hex>(heuristicComparison);
        frontier.Add(start);

        HashSet<Hex> visited = new HashSet<Hex>();
        visited.Add(start);

        start.PrevTile = null;

        while (frontier.Count > 0)
        {
            Hex current = frontier.Remove();
            if (current == end)
            {
               
                break;
            }

            foreach (var neighbor in grid.GetNeighbors(current))
            {

                

                    int newNeighborCost = current.Cost + neighbor.Weight;
                    if (newNeighborCost < neighbor.Cost)
                    {
                        neighbor.Cost = newNeighborCost;
                        neighbor.PrevTile = current;

                    }

                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        frontier.Add(neighbor);


                    }
                
            }
        }
        return end.Cost <= range;
    }
    public static List<Hex> BFS_ListInRange(HexMap grid, Hex start, float MoveRange)
    {
        foreach (var tile in grid.hexes)
        {
            tile.Cost = int.MaxValue;
        }

        start.Cost = 0;

        HashSet<Hex> visited = new HashSet<Hex>();
        visited.Add(start);

        Queue<Hex> frontier = new Queue<Hex>();
        frontier.Enqueue(start);

        start.PrevTile = null;
        List<Hex> tilesInRange = new List<Hex>();
        while (frontier.Count > 0)
        {
            Hex current = frontier.Dequeue();


            foreach (var neighbor in grid.GetNeighbors(current))
            {
                

                    int newNeighborCost = current.Cost + neighbor.Weight;


                    if (newNeighborCost < neighbor.Cost)
                    {

                        neighbor.Cost = newNeighborCost;
                        neighbor.PrevTile = current;

                    }


                    if (!visited.Contains(neighbor))
                    {
                        if (MoveRange - newNeighborCost >= 0)
                        {
                            tilesInRange.Add(neighbor);
                        }
                        else
                        {
                            return tilesInRange;
                        }
                        visited.Add(neighbor);
                        frontier.Enqueue(neighbor);


                    }
                
            }
        }




        return tilesInRange;
    }
    public static Boolean BFS_ShowRange(HexMap grid, Hex start, float range, Color color)
    {
        foreach (var tile in grid.hexes)
        {
            tile.Cost = int.MaxValue;
        }

        start.Cost = 0;

        HashSet<Hex> visited = new HashSet<Hex>();
        visited.Add(start);

        Queue<Hex> frontier = new Queue<Hex>();
        frontier.Enqueue(start);

        start.PrevTile = null;
        while (frontier.Count > 0)
        {
            Hex current = frontier.Dequeue();

            foreach (var neighbor in grid.GetNeighbors(current))
            {
                int newNeighborCost = current.Cost + neighbor.Weight;


                if (newNeighborCost < neighbor.Cost)
                {

                    neighbor.Cost = newNeighborCost;
                    neighbor.PrevTile = current;

                }


                if (!visited.Contains(neighbor))
                {
                    if (range - newNeighborCost >= 0)
                    {
                        neighbor.SetColor(color);
                    }
                    else
                    {
                        return true;
                    }
                    visited.Add(neighbor);
                    frontier.Enqueue(neighbor);


                }
            }
        }

        return false;
    }

    private static float GetEuclideanHeuristicCost(Hex current, Hex end)
    {
        float heuristicCost = (current.ToVector3() - end.ToVector3()).magnitude;
        return heuristicCost;
    }

    private static List<Hex> BacktrackToPath(Hex end, HexMap grid)
    {
        Hex current = end;
        List<Hex> path = new List<Hex>();
        while (current != null)
        {
           
            path.Add(current);
            current = current.PrevTile;
        }

        path.Reverse();
      

        return path;
    }
}

