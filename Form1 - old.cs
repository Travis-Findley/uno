using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace zuno
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void button1_Click_1(object sender, EventArgs e)
		{
			gamelogic game = new gamelogic(this.Width, this.Height);
			foreach (player p in game.players)
			{
				p.makepicturearea();
				this.Controls.Add(p.LB);
				foreach (card c in p.deck)
				{
					c.setimage();
					this.Controls.Add(c.picture);
				}
			}
		}
	}

	class card
	{
		public string[] color = {"", ""};
		public string[] number = {"", ""};
		public int[] points = {-1, -1};
		public PictureBox picture = new PictureBox();
		public int[] ailevel = {-1, -1};
		//x, y, changed
		public int[] loctaion = {-1, -1, -1};
		bool isflipped = false;
		public card(string[] color, string[] number, int[] points)
		{
			this.color = color;
			this.number = number;
			this.points = points;
			this.ailevel = points;
		}

		public string getname()
		{
			int i = Convert.ToInt32(isflipped);
			return $"small//{color[i]}_{number[i]}.png";
		}

		public void setimage()
		{
			picture.Image = Image.FromFile(Application.StartupPath + "\\" + this.getname());
			picture.Location = new Point(loctaion[0], loctaion[1]);
			picture.Name = "pictureBox1";
			picture.Size = new Size(100, 100);
			picture.TabStop = false;
			picture.Click += new EventHandler(this.picture_Click);
		}

		public void update(bool isflipped)
		{
			this.isflipped = isflipped;
		}

		private void picture_Click(object sender, EventArgs e)
		{
		}
	}

	class discardpile
	{
		public List<List<card>> discardpilelist = new List<List<card>>();
		public bool isflipped = false;
		public bool playcard(card c, List<List<card>> deck, bool isflipped)
		{
			this.isflipped = isflipped;
			int i = RandomNumber.flipnumber(isflipped);
			List<card> eligablecards = cardvalues.eligablecards(deck[i], c, isflipped);
			if (eligablecards.Contains(c))
			{
				discardpilelist[i].Remove(c);
				return true;
			}

			return false;
		}

		private int[] findaddcard(card topcard, int pastadd)
		{
			int[] ints = {-1, pastadd};
			if (!isflipped && (topcard.number[0] == "+4" || topcard.number[0] == "+2"))
			{
				ints = new int[]{1, int.Parse(topcard.number[0]) + pastadd};
			}

			return ints;
		}

		public void update(bool isflipped)
		{
			this.isflipped = isflipped;
		}
	}

	class player
	{
		public List<card> deck = new List<card>();
		public bool aicontroled;
		public string name;
		public int[] startinglocation = {-1, -1};
		public System.Windows.Forms.Label LB = new System.Windows.Forms.Label();
		private int[] LBlocation = new int[2];
		private bool isflipped = false;
		public player(bool aicontroled, string name, int[] startinglocation, int LBindex)
		{
			this.aicontroled = aicontroled;
			this.name = name;
			this.startinglocation = startinglocation;
			LBlocation = cardvalues.LBlocations[LBindex];
			this.LB.AutoSize = true;
			this.LB.Location = new Point(LBlocation[0], LBlocation[1]);
			this.LB.Name = "player1LB";
			this.LB.Text = $"{aicontroled}, {LBlocation[0]}, {LBlocation[1]}";
		}

		public string readout(string type)
		{
			string word = "";
			if (type == "cards")
			{
				foreach (card c in deck)
				{
					word += $"({c.color[0]}, {c.number[0]}), ";
				}
			}

			if (type == "points")
			{
				foreach (card c in deck)
				{
					word += $"({c.points[0]}), ";
				}
			}

			return word;
		}

		// picture size 100, 132
		public void makepicturearea()
		{
			for (int i = 0; i < deck.Count; i++)
			{
				if (i < deck.Count / 2)
				{
					if ((deck[i].loctaion[deck[i].loctaion[2]] -= (deck.Count - i) * 100) < 0)
					{
						deck[i].loctaion[deck[i].loctaion[2]] -= (deck.Count - i) * 50;
					}
					else
					{
						deck[i].loctaion[deck[i].loctaion[2]] -= (deck.Count - i) * 100;
					}
				}
				else if (i > deck.Count / 2)
				{
					if ((deck[i].loctaion[deck[i].loctaion[2]] += (deck.Count - i) * 100) < 0)
					{
						deck[i].loctaion[deck[i].loctaion[2]] -= (deck.Count - i) * 50;
					}
					else
					{
						deck[i].loctaion[deck[i].loctaion[2]] += (deck.Count - i) * 100;
					}
				}
			}
		}

		public void renderpictures()
		{
		}

		public bool update(bool isflipped)
		{
			if (deck.Count == 0)
			{
				return true;
			}

			LB.Text = deck.Count.ToString();
			this.isflipped = isflipped;
			foreach (card c in deck)
			{
				c.update(isflipped);
			}

			return false;
		}

		public card play()
		{
			//add on click event
			return deck[0];
		}
	}

	class aicontroler
	{
		public aicontroler()
		{
		}

		public void findplay()
		{
		}
	}

	class cardvalues
	{
		public static string[, ] colors = {{"red", "yellow", "green", "blue", "red", "yellow", "green", "blue"}, {"purple", "teal", "orange", "pink", "purple", "teal", "orange", "pink"}};
		public static string[, ] numbers = {{"0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "+2", "reverse", "skip"}};
		public static string[, ] wilds = {{"wild", "+4"}}; //, "rotate decks"
		public static int[, ] points = {{1, 2, 3, 4, 5, 6, 7, 8, 9, 12, 20, 25}, {1, 2, 3, 4, 5, 6, 7, 8, 9, -1, -1, -1}, {30, 50, 45, -1, -1, -1, -1, -1, -1, -1, -1, -1}};
		public static List<int[]> LBlocations = new List<int[]>()
		{new int[]{162, 622}, new int[]{1022, 622}, new int[]{1022, 118}, new int[]{162, 118}};
		public List<card> sort(string type, bool isflipped, List<card> deck)
		{
			int index = RandomNumber.flipnumber(isflipped);
			if (type == "color")
			{
				List<List<card>> newlist = this.splitbycolor(index, deck);
				foreach (List<card> l in newlist)
				{
					foreach (card c in l)
					{
						deck.Add(c);
					}
				}
			}
			else if (type == "points")
			{
				deck = this.sortpoints(index, deck);
			}
			else if (type == "both")
			{
				List<List<card>> newlist = this.splitbycolor(index, deck);
				for (int i = 0; i < 4; i++)
				{
					newlist[i] = this.sortpoints(index, newlist[i]);
				}

				foreach (List<card> l in newlist)
				{
					foreach (card c in l)
					{
						deck.Add(c);
					}
				}
			}

			return deck;
		}

		private List<card> sortpoints(int j, List<card> deck)
		{
			List<card> newlist = new List<card>();
			bool passed = false;
			while (!passed)
			{
				passed = true;
				for (int i = 1; i < deck.Count; i++)
				{
					if (newlist[i - 1].points[0] < newlist[i].points[0])
					{
						passed = false;
						card temp = newlist[i - 1];
						newlist[i - 1] = newlist[i];
						newlist[i] = temp;
					}
				}
			}

			return newlist;
		}

		private List<List<card>> splitbycolor(int index, List<card> deck)
		{
			List<string> colors = new List<string>()
			{"red", "yellow", "green", "blue", "wild"};
			List<List<card>> newlist = new List<List<card>>()
			{new List<card>(), new List<card>(), new List<card>(), new List<card>(), new List<card>()};
			while (deck.Count > 0)
			{
				newlist[colors.IndexOf(deck[0].color[index])].Add(deck[0]);
				deck.RemoveAt(0);
			}

			return newlist;
		}

		public static List<card> eligablecards(List<card> deck, card topofdeck, bool isflipped)
		{
			List<card> eligablecards = new List<card>();
			int i = RandomNumber.flipnumber(isflipped);
			for (int j = 0; j < deck.Count; j++)
			{
				if (deck[j].color[i] == "wild" || deck[j].color[i] == topofdeck.color[i] || deck[j].number[i] == topofdeck.number[i])
				{
					eligablecards.Add(deck[j]);
				}
			}

			return eligablecards;
		}
	}

	class gamelogic
	{
		//game rules
		public bool doflipped = false;
		public bool drawtomatchbool = true;
		private int amountofplayers = 4;
		private List<List<card>> deck = new List<List<card>>();
		public List<player> players = new List<player>();
        //game sets
		public bool isflipped = false;
		private bool isreversed = false;
		private int playerindex = 0;
		private int flipnumber;

		public gamelogic(int width, int height)
		{
			//Bottem Top Right Left
			List<int[]> startinglocation = new List<int[]>()
			{new int[]{width / 2, height - 100, 0}, new int[]{width / 2, 100, 0}, new int[]{100, height / 2, 1}, new int[]{width - 100, height / 2, 1}};
			for (int i = 0; i < cardvalues.colors[0, 0].Length; i++)
			{
				for (int j = 0; j < 2; j++)
				{
					for (int x = j; x < 12; x++)
					{
						string[] newcolors = new string[]{cardvalues.colors[0, i], ""};
						string[] newnumbers = {cardvalues.numbers[0, x], ""};
						int[] newpoints = {cardvalues.points[0, x], -1};
						deck.Add(new card(newcolors, newnumbers, newpoints));
					}
				}
			}

			for (int i = 0; i < 2; i++)
			{
				string[] color = {"wild", "wild"};
				string[] number = {cardvalues.wilds[0, i], ""};
				int[] points = {cardvalues.points[2, i], -1};
				deck.Add(new card(color, number, points));
			}

			players.Add(new player(false, "noai", startinglocation[0], 0));
			for (int i = 1; i < amountofplayers; i++)
			{
				players.Add(new player(true, "yesai", startinglocation[i], i));
			}

			for (int i = 0; i < players.Count; i++)
			{
				for (int j = 0; j < 10; j++)
				{
					card addcard = deck[flipnumber][RandomNumber.Between(0, deck.Count)];
					players[i].deck.Add(addcard);
					deck.Remove(addcard);
				}
			}
		}

		private List<List<card>> drawtomatch(player pl, card topofdeck)
		{
			card newcard = deck[flipnumber][RandomNumber.Between(0, deck.Count)];
			List<List<card>> addlist = new List<List<card>>();
			int i = RandomNumber.flipnumber(isflipped);
			while (newcard.color[i] != "wild" || newcard.color[i] != topofdeck.color[i] || newcard.number[i] != topofdeck.number[i])
			{
				addlist.Add(newcard);
				newcard = deck[flipnumber][RandomNumber.Between(0, deck.Count)];
			}

			return addlist;
		}

		public void drawcard(player pl, card topofdeck)
		{
			if (drawtomatchbool)
			{
				pl.deck = this.drawtomatch(pl, topofdeck, isflipped);
			}
			else
			{
				pl.deck.Add(deck[flipnumber][RandomNumber.Between(0, deck.Count)]);
			}
		}

		private void nextplayer()
		{
			if (isreversed && playerindex++ < players.Count)
			{
				playerindex++;
			}
			else if (isreversed && playerindex++ == players.Count)
			{
				playerindex = 0;
			}
			else if (!isreversed && playerindex-- > 0)
			{
				playerindex--;
			}
			else if (!isreversed && playerindex-- == 0)
			{
				playerindex = players.Count - 1;
			}
		}

		public void playturn()
		{
			foreach (player p in players)
			{
				bool won = p.update();
				if (won)
				{
					this.won(p);
					return;
				}
			}

			this.nextplayer();
		}

		public void won(player p)
		{
		}
	}

	public static class RandomNumber
	{
		private static readonly RNGCryptoServiceProvider _generator = new RNGCryptoServiceProvider();
		public static int Between(int minimumValue, int maximumValue)
		{
			byte[] randomNumber = new byte[1];
			_generator.GetBytes(randomNumber);
			double asciiValueOfRandomCharacter = Convert.ToDouble(randomNumber[0]);
			double multiplier = Math.Max(0, (asciiValueOfRandomCharacter / 255d) - 0.00000000001d);
			int range = maximumValue - minimumValue;
			double randomValueInRange = Math.Floor(multiplier * range);
			return (int)(minimumValue + randomValueInRange);
		}
	}
}