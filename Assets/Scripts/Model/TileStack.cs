/*
Class tile
TODO
*/



public class TileStack{

    public bool water;
    public bool tree;
    public bool grass;
    public bool building;
    public bool ground;
    public bool rock;
    public bool material;

    public TileStack(TileType[] tilesType){
       if (tilesType.Length == 1 && tilesType[0] == TileType.water){
        water = true;
       }
        //TODO
    }

    public override string ToString()
        {
            if (water){
                return "This is some good water";
            }
            return "No watel fol you my fliend";
        }
}