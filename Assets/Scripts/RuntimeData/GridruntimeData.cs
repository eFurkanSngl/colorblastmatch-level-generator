namespace Assets.Scripts.RuntimeData
{
    public  class GridruntimeData
    {
        #region Game Data
        private readonly CellData[,] cellData;
        
        public int Width {  get; private set; }
        public int Height { get; private set; }

        #endregion

      
        public GridruntimeData(int GridX , int GridY)
        {
            Width = GridX;
            Height = GridY;

            cellData = new CellData[Width, Height];

            GenerateGrid();
        }

        private void GenerateGrid()
        {
            for(int i = 0; i < Width; i++)
            {
                for(int j = 0; j < Height; j++)
                {
                    CellData cell = new CellData(i, j);
                    cellData[i, j] = cell;
                }
            }
        }
        
        public CellData GetCell(int x , int y)
        {
            if (!IsInside(x, y)) return null;
            return cellData[x, y];
        }

        public void SetCell(int x , int y , CellData cell)
        {
            if (!IsInside(x, y)) return;

            cellData[x, y] = cell;
        }

        public CellData GetNeighbour(int x, int y, int offsetX, int offsetY)
        {
            return GetCell(x + offsetX, y + offsetY);
        }
        private bool IsInside(int x , int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

    }
}
