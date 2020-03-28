using System;
using System.Collections.Generic;
using System.Linq;

namespace PairAndImagesLibrary
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Welcome to test harness for Image Clue Game");
            Console.WriteLine(string.Format("Will generate teams and clues for {0} players", args.Length));
            List<List<string>> teams = PairAndImagesLibraryMain.GetTeams(args.ToList());
            List<Clue> clues = PairAndImagesLibraryMain.GetClues(teams);
            
            for (int index = 0; index < clues.Count; index++)
            {
                Console.WriteLine(string.Format("Team {0} is: {1}.", index + 1, string.Join(',', teams[index])));
                Console.WriteLine(string.Format("Their clue is: {0}", clues[index].ToString()));
                Console.ReadLine();
            }
            Console.WriteLine("That's all for now!");
            Console.ReadLine();
        }
    }
}
