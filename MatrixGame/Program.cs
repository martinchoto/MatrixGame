using MatrixGame.Database;
using MatrixGame.Database.DbModels;
using MatrixGame.Enums;
using MatrixGame.Models;

char[,] grid = new char[10, 10];
int playerRow = -1;
int playerCol = -1;
MatrixInit(grid);
Character character = null;
MatrixGameDbContext dbContext = new MatrixGameDbContext();
Dictionary<(int, int), Monster> monstersByPostion = new Dictionary<(int, int), Monster>();

StartMenu();
CharacterSelect();
InGame();

void InGame()
{
	Console.Title = Screens.InGame.ToString();
	Console.Clear();
	Console.WriteLine($"Health: {character.Health}		Mana: {character.Mana}\n");
	PrintMatrix(grid);
	Console.WriteLine("Choose an action:\n1) Attack\n2) Move");
	int action;
	bool success = int.TryParse(Console.ReadLine(), out action);
	if (!success || (action < 1 || action > 2))
	{
		InGame();
		return;
	}
	switch (action)
	{
		case 1:
			Attack();
			break;
		case 2:
			Move();
			break;
	}
	InGame();
	return;
}
void MonstersAttack()
{
	foreach (var kvp in monstersByPostion)
	{
		var (mx, my) = kvp.Key;
		var monster = kvp.Value;

		int dx = Math.Abs(playerRow - mx);
		int dy = Math.Abs(playerCol - my);

		if (dx <= monster.Range && dy <= monster.Range)
		{
			character.Health -= monster.Damage;
			if (character.Health <= 0)
				character.Health = 0;
			Console.WriteLine($"Monster at ({mx},{my}) attacks you for {monster.Damage} damage! Your health is now {character.Health}");
			CheckPlayerDeath();
		}
	}

	Console.ReadKey();
}
void Attack()
{
	List<((int x, int y), Monster)> targetsInRange = new();
	for (int dx = -character.Range; dx <= character.Range; dx++)
	{
		for (int dy = -character.Range; dy <= character.Range; dy++)
		{
			if (dx == 0 && dy == 0)
				continue;

			int newX = playerRow + dx;
			int newY = playerCol + dy;

			if (newX >= 0 && newX < grid.GetLength(0) && newY >= 0 && newY < grid.GetLength(1))
			{
				if (monstersByPostion.TryGetValue((newX, newY), out var monster))
				{
					targetsInRange.Add(((newX, newY), monster));
				}
			}
		}
	}
	if (targetsInRange.Count == 0)
	{
		Console.WriteLine("No monsters in range to attack.");
		Console.WriteLine("Press any key to continue...");
		Console.ReadKey();
		return;
	}
	Console.WriteLine("Available targets:");
	for (int i = 0; i < targetsInRange.Count; i++)
	{
		var ((x, y), m) = targetsInRange[i];
		Console.WriteLine($"{i + 1}. Monster at ({x},{y}) - HP: {m.Health}");
	}

	Console.Write("Choose a monster to attack by number: ");
	if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > targetsInRange.Count)
	{
		Console.WriteLine("Invalid choice.");
		Console.WriteLine("Press any key to continue...");
		Console.ReadKey();
		return;
	}

	var ((targetX, targetY), targetMonster) = targetsInRange[choice - 1];
	targetMonster.Health -= character.Damage;
	Console.WriteLine($"You dealt {character.Damage} damage to monster at ({targetX},{targetY})");

	if (targetMonster.Health <= 0)
	{
		Console.WriteLine("Monster defeated!");
		monstersByPostion.Remove((targetX, targetY));
		grid[targetX, targetY] = '▒';
	}
	MonstersAttack();
	MoveMonstersTowardsPlayer();
	Console.WriteLine("Press any key to continue...");
	Console.ReadKey();
}

void Move()
{
	Console.WriteLine("Use W/A/S/D or Q/E/Z/X to move. Press ESC to exit.");
	ConsoleKeyInfo keyInfo = Console.ReadKey();
	int newPlayerRow = playerRow, newPlayerCol = playerCol;

	switch (keyInfo.Key)
	{
		case ConsoleKey.W: newPlayerRow = playerRow - 1; break;
		case ConsoleKey.S: newPlayerRow = playerRow + 1; break;
		case ConsoleKey.A: newPlayerCol = playerCol - 1; break;
		case ConsoleKey.D: newPlayerCol = playerCol + 1; break;
		case ConsoleKey.Q: newPlayerCol = playerCol - 1; newPlayerRow = playerRow - 1; break;
		case ConsoleKey.E: newPlayerCol = playerCol + 1; newPlayerRow = playerRow - 1; break;
		case ConsoleKey.Z: newPlayerCol = playerCol - 1; newPlayerRow = playerRow + 1; break;
		case ConsoleKey.X: newPlayerCol = playerCol + 1; newPlayerRow = playerRow + 1; break;
		case ConsoleKey.Escape:
			Console.WriteLine("Exiting game. Thank you for playing!");
			Console.Title = Screens.Exit.ToString();
			Console.Clear();
			Console.WriteLine("Press any key to exit...");
			Console.ReadKey();
			Environment.Exit(0);
			return;
		default:
			Console.WriteLine("Invalid key! Use W/A/S/D or Q/E/Z/X to move.");
			Console.WriteLine("Press any key to continue...");
			Console.ReadKey();
			Move();
			break;
	}

	if (newPlayerCol >= 0 && newPlayerCol < grid.GetLength(1) && newPlayerRow >= 0 && newPlayerRow < grid.GetLength(0))
	{
		if (grid[newPlayerRow, newPlayerCol] == 'M')
		{
			Console.WriteLine("You cannot move while a monster is on your position!");
			Console.WriteLine("Press any key to continue...");
			Console.ReadKey();
			return;
		}
		grid[playerRow, playerCol] = '▒';
		playerRow = newPlayerRow;
		playerCol = newPlayerCol;
		grid[playerRow, playerCol] = character.Symbol;
	}
	else
	{
		Console.WriteLine("Invalid move! You cannot go outside the grid.");
		Console.WriteLine("Press any key to continue...");
		Console.ReadKey();
		return;
	}
	MonstersAttack();
	MoveMonstersTowardsPlayer();
	SpawnMonster();
	InGame();
	void SpawnMonster()
	{
		Monster monster = new Monster();
		int monsterRow, monsterCol;
		Random rand = new Random();

		do
		{
			monsterRow = rand.Next(0, grid.GetLength(0));
			monsterCol = rand.Next(0, grid.GetLength(1));
		} while (grid[monsterRow, monsterCol] != '▒');

		grid[monsterRow, monsterCol] = monster.Symbol;
		monster.SetUp();
		if (monstersByPostion.ContainsKey((monsterRow, monsterCol)))
		{
			SpawnMonster();
			return;
		}
		monstersByPostion.Add((monsterRow, monsterCol), monster);
	}
}
void MoveMonstersTowardsPlayer()
{
	var updatedPositions = new Dictionary<(int, int), Monster>();

	foreach (var kvp in monstersByPostion)
	{
		var (mx, my) = kvp.Key;
		var monster = kvp.Value;

		int dx = Math.Sign(playerRow - mx);
		int dy = Math.Sign(playerCol - my);
		grid[mx, my] = '▒';
		int newX = mx + dx;
		int newY = my + dy;

		if (newX >= 0 && newX < grid.GetLength(0) &&
			newY >= 0 && newY < grid.GetLength(1) &&
			!monstersByPostion.ContainsKey((newX, newY)) &&
			!(newX == playerRow && newY == playerCol) && grid[playerRow, playerCol] == character.Symbol)
		{
			updatedPositions[(newX, newY)] = monster;
			grid[newX, newY] = 'M';
		}
		else
		{
			updatedPositions[(mx, my)] = monster;
			grid[mx, my] = 'M';
		}
	}

	monstersByPostion.Clear();
	foreach (var entry in updatedPositions)
		monstersByPostion[entry.Key] = entry.Value;
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
	playerRow = 0;
	playerCol = 0;
	switch (choice)
	{
		case 1:
			character = new Warrior();
			grid[playerRow, playerCol] = character.Symbol;
			break;
		case 2:
			character = new Archer();
			grid[playerRow, playerCol] = character.Symbol;
			break;
		case 3:
			character = new Mage();
			grid[playerRow, playerCol] = character.Symbol;
			break;
	}
	AddCharacterStats(character);
	character.SetUp();
	//AddCharToDb(character);


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
void CheckPlayerDeath()
{
	if (character.Health <= 0)
	{
		Console.WriteLine("You have been defeated! Game Over.");
		Console.WriteLine("Press any key to exit...");
		Console.ReadKey();
		Console.Title = Screens.Exit.ToString();
		Console.Clear();
		Console.WriteLine("Thank you for playing!");
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