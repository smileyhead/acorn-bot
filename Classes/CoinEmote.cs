namespace Acorn.Classes
{
    public class CoinEmote
    {
        public string GetEmote(bool isHeads)
        {
            Random random = new();

            if (isHeads)
            {
                string[] heads =
                [
                    "<:5forint_heads:1355978849078477052>",
                    "<:10forint_heads:1355978872860049598>",
                    "<:20forint_heads:1355978891583295678>",
                    "<:50forint_heads:1355978908415299625>",
                    "<:100forint_heads:1355978943290806303>",
                    "<:200forint_heads:1355979000064901280>"
                ];
                return heads[random.Next(heads.Length)];
            }

            string[] tails =
            [
                "<:5forint_tails:1355978862697254922>",
                "<:10forint_tails:1355978881777008820>",
                "<:20forint_tails:1355978900362232009>",
                "<:50forint_tails:1355978928979841094>",
                "<:100forint_tails:1355978988337758390>",
                "<:200forint_tails:1355979016665829606>"
            ];
            return tails[random.Next(tails.Length)];
        }
    }
}
