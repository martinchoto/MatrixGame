char[,] grid = new char[10, 10];
MatrixInit(grid);
PrintMatrix(grid);

void MatrixInit(char[,] grid)
{
	for (int i = 0; i < grid.GetLength(0); i++)
	{
		for (int j = 0; j < grid.GetLength(1); j++)
		{
			grid[i, j] = '▒';
		}
	}
}
void PrintMatrix(char[,] grid)
{
	for (int i = 0; i < grid.GetLength(0); i++)
	{
		for (int j = 0; j < grid.GetLength(1); j++)
		{
			Console.Write(grid[i, j] + " ");
		}
		Console.WriteLine();
	}
}