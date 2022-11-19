/*
Class tile
TODO
*/



public class TileStack
{

    public bool water;
    public bool tree;
    public bool grass;
    public bool building; //TODO
    public bool ground;
    public bool rock;
    public bool metal;
    public bool gold;
    public bool uranium;

    public TileStack(TileType[] tilesType)
    {
        for (int i = 0; i < tilesType.Length; i++)
        {
            switch (tilesType[i])
            {
                case TileType.water:
                    water = true;
                    break;
                case TileType.tree:
                    tree = true;
                    break;
                case TileType.grass:
                    grass = true;
                    break;
                case TileType.ground:
                    ground = true;
                    break;
                case TileType.rock:
                    rock = true;
                    break;
                case TileType.metal:
                    metal = true;
                    break;
                case TileType.gold:
                    gold = true;
                    break;
                case TileType.uranium:
                    uranium = true;
                    break;
                default:
                    break;
            }
        }
    }

    public override string ToString()
    {
        if (water)
        {
            return "This is some good water";
        }
        return "No watel fol you my fliend";
    }
}