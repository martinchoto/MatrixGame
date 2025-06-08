using MatrixGame.Database;
using MatrixGame.Database.DbModels;
using MatrixGame.Enums;
using MatrixGame.Models;

char[,] grid = new char[10, 10];
MatrixInit(grid);
Character character = null;
MatrixGameDbContext dbContext = new MatrixGameDbContext();

StartMenu();
CharacterSelect();
InGame();

void InGame()
{
	Console.Title = Screens.InGame.ToString();
	Console.Clear();
	Console.WriteLine($"Health: {character.Health}		Mana: {character.Mana}\n\n");
	PrintMatrix(grid);
	Console.WriteLine("Choose an action:\n1) Attack\n2) Move)");
	int action;
	bool success = int.TryParse(Console.ReadLine(), out action);
	if (!success || (action < 1 || action > 2))
	{
		Console.WriteLine("Invalid choice. Please select a valid action.");
		InGame();
		return;
	}


}

void StartMenu()
{
	Console.Title = Screens.MainMenu.ToString();
	Console.WriteLine("Welcome\nPress any key to play.");
	Console.ReadKey();
	Console.Clear();
}
void CharacterSelect()
{
	Console.Title = Screens.CharacterSelect.ToString();	
	Console.WriteLine("Choose character type:\n Options:");
	Console.WriteLine("1) Warrior\n2) Archer\n3) Mage\nYour pick:");
	int choice;
	bool success = int.TryParse(Console.ReadLine(), out choice);
	if (!success || choice < 1 || choice > 3)
	{
		Console.WriteLine("Invalid choice. Please select a valid character type.");
		CharacterSelect();
		return;
	}
	switch (choice)
	{
		case 1:
			character = new Warrior();
			grid[0, 0] = character.Symbol;
			break;
		case 2:
			character = new Archer();
			grid[0, 0] = character.Symbol;
			break;
		case 3:
			character = new Mage();
			grid[0, 0] = character.Symbol;
			break;
	}
	AddCharacterStats(character);
	character.SetUp();
	AddCharToDb(character);


	void AddCharacterStats(Character character)
	{
		char yesOrNo;
		Console.WriteLine("Would you like to buff up your stats before starting?		(Limit: 3 points total)");
		Console.Write("Response (Y/N):");
		bool validInput = char.TryParse(Console.ReadLine(), out yesOrNo);
		yesOrNo = char.ToLower(yesOrNo);
		if (!validInput || (yesOrNo != 'y' && yesOrNo != 'n'))
		{
			Console.WriteLine("Invalid input. Please enter 'y' for yes or 'n' for no.");
			AddCharacterStats(character);
			return;
		}
		int remainingPoints = 3;
		if (yesOrNo == 'y')
		{
			Console.Write("Add to Strenght: ");
			int strengthPoints = int.TryParse(Console.ReadLine(), out strengthPoints) ? strengthPoints : 0;
			if (strengthPoints < 0 || strengthPoints > remainingPoints)
			{
				Console.WriteLine("Invalid input. Please enter a number between 0 and " + remainingPoints);
				AddCharacterStats(character);
				return;
			}
			remainingPoints -= strengthPoints;
			character.Strength += strengthPoints;
			if (remainingPoints <= 0)
			{
				return;
			}
			Console.Write("Add to Agility: ");
			int agilityPoints = int.TryParse(Console.ReadLine(), out agilityPoints) ? agilityPoints : 0;
			if (agilityPoints < 0 || agilityPoints > remainingPoints)
			{
				Console.WriteLine("Invalid input. Please enter a number between 0 and " + remainingPoints);
				AddCharacterStats(character);
				return;
			}
			remainingPoints -= agilityPoints;
			character.Agility += agilityPoints;
			if (remainingPoints <= 0)
			{
				return;
			}
			Console.Write("Add to Intelligence: ");
			int intelligencePoints = int.TryParse(Console.ReadLine(), out intelligencePoints) ? intelligencePoints : 0;
			if (intelligencePoints < 0 || intelligencePoints > remainingPoints)
			{
				Console.WriteLine("Invalid input. Please enter a number between 0 and " + remainingPoints);
				AddCharacterStats(character);
				return;
			}
			remainingPoints -= intelligencePoints;
			character.Intelligence += intelligencePoints;
			if (remainingPoints <= 0)
			{
				return;
			}
		}
	}
	void AddCharToDb(Character character)
	{
		PlayableCharacter plChar = new PlayableCharacter()
		{
			Class = character.GetType().Name,
			Health = character.Health,
			Mana = character.Mana,
			Damage = character.Damage,
			Strength = character.Strength,
			Agility = character.Agility,
			Intelligence = character.Intelligence,
			Range = character.Range,
		};
		dbContext.PlayableCharacters.Add(plChar);
		dbContext.SaveChanges();
	}
}

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
			Console.Write(grid[i, j]);
		}
		Console.WriteLine();
	}
}