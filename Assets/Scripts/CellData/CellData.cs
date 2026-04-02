using UnityEngine;

public class CellData 
{
    public BlockBase block;
    public bool IsOccupied => block != null;
    public int gridX { get;}
    public int gridY { get; }
    public CellData(int x , int y)
    {
        gridX = x;
        gridY = y;
    }

    public void SetBlock(BlockBase currentBlock)
    {
        block = currentBlock;

        if(currentBlock != null)
        {
            block.SetCell(this);
        }
    }

    public void ClearBlock()
    {
        if (block != null)
            block.ClearCell();

        block = null;
    }

}
