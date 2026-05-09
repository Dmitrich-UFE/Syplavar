using UnityEngine;

public class TreeData
{
    internal int ID {get; private set;}
    internal int Phase {get; private set;}
    internal int Type {get; private set;}
    internal PlantTree Tree {get; private set;}

    internal TreeData(int id, int phase, int type, PlantTree tree)
    {
        ID = id;
        Phase = phase;
        Type = type;
        Tree = tree;
    }

    internal void ReInit(int id, int phase, int type, PlantTree tree)
    {
        ID = id;
        Phase = phase;
        Type = type;
        Tree = tree;
    }
}
