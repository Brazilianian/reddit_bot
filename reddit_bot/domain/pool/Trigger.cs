namespace reddit_bor.domain.pool
{
    public class Trigger
    {
        public string Text { get; set; }
        public Place Place { get; set; }

        public Trigger(string text, Place place)
        {
            Text = text;
            Place = place;
        }

        public Trigger(string text)
        {
            Text = text;
        }

        public Trigger()
        {
        }

        public override string ToString()
        {
            string trigger = "";
            switch (Place)
            {
                case Place.Start:
                    trigger = "(П) ";
                    break;
                case Place.Middle:
                    trigger = "(С) ";
                    break;
                case Place.End:
                    trigger = "(К) ";
                    break;
            }

            return trigger += Text;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
