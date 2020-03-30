namespace PairAndImagesLibrary
{
    public class Clue
    {
        public string Adjective { get; set; }
        public string Noun { get; set; }

        public Clue(string adjective, string noun)
        {
            Adjective = adjective;
            Noun = noun;
        }

        public override string ToString()
        {
            return string.Format("Draw a {0} {1}", Adjective, Noun);
        }
    }
}