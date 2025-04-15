using Acorn.Records;
using Newtonsoft.Json;
using System.Globalization;
using System.Text.Json;

namespace Acorn.Classes
{
    internal class Exchange
    {
        private List<Currency> Currencies;
        private DateTime LastUpdated;

        public Exchange()
        {
            Console.WriteLine("  Compiling currencies.");

            Currencies = new();

            Update();
            Read();
        }

        private void Update()
        {
            if (!File.Exists(Program.currencyPath))
            {
                Console.WriteLine("  Currency database doesn't exist. Downloading.");
                Download();
                return;
            }

            if ((DateTime.Now - File.GetLastWriteTime(Program.currencyPath)).TotalHours >= 26)
            {
                Console.WriteLine("  Currency database deemed too old. Updating.");
                Download();
                return;
            }

            Console.WriteLine("  Currency database exists and is more recent than 26 hours. Opting not to update.");
        }

        private void Download()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    using (var s = client.GetStreamAsync("https://cdn.jsdelivr.net/npm/@fawazahmed0/currency-api@latest/v1/currencies/eur.json"))
                    {
                        using (var fs = new FileStream(Program.currencyPath, FileMode.OpenOrCreate)) { fs.SetLength(0); s.Result.CopyTo(fs); }
                    }
                }
                catch (HttpRequestException e) { Console.WriteLine($"Halting update – HTTP exception: {e}"); }
            }
        }

        public string DoExchange(string inputValue, string inputCurrency, string outputCurrency)
        {
            bool success;
            string outputValue;
            string inputName;
            string outputName;

            if (!Double.TryParse(inputValue, CultureInfo.InvariantCulture, out double value)) { return "Error: The input value could not be parsed."; }

            if (outputCurrency != "All")
            {
                (success, outputValue, inputName, outputName) = Convert(value, inputCurrency, outputCurrency);
                if (!success) { return outputName; }

                return $"**{value} {inputName.ToString(CultureInfo.CreateSpecificCulture("en-US"))}** equals **{outputValue} {outputName}**.\n" +
                       $"-# Rates based on data from <t:{((DateTimeOffset)LastUpdated).ToUnixTimeSeconds()}:D>.";
            }

            List<string> outputCurrencies = ["CAD", "CRC", "EUR", "HUF", "IDR", "JPY", "KZT", "PHP", "USD", "VND"];
            outputCurrencies.Remove(outputCurrencies.Find(c => c == inputCurrency));

            (success, outputValue, inputName, outputName) = Convert(value, inputCurrency, outputCurrencies[0]);
            if (!success) { return outputName; }

            string answer = $"**{value} {inputName.ToString(CultureInfo.CreateSpecificCulture("en-US"))}** equals…\n" +
                            $"- **{outputValue}** {outputName}\n";

            for (int i = 1; i < outputCurrencies.Count; i++)
            {
                (success, outputValue, inputName, outputName) = Convert(value, inputCurrency, outputCurrencies[i]);
                if (!success) { return outputName; }
                answer += $"- **{outputValue}** {outputName}\n";
            }

            answer += $"-# Rates based on data from <t:{((DateTimeOffset)LastUpdated).ToUnixTimeSeconds()}:D>.";

            return answer;
        }

        private (bool success, string outputValue, string inputName, string outputName)
            Convert(double value, string inputCurrencyCode, string outputCurrencyCode)
        {
            Currency? inputCurrency = Currencies.Find(i => i.Code == inputCurrencyCode);
            if (inputCurrency == null) { return (false, "", "", "Error: The input currency could not be found."); }
            double inputValue = inputCurrency.Value;

            Currency? outputCurrency = Currencies.Find(i => i.Code == outputCurrencyCode);
            if (outputCurrency == null) { return (false, "", "", "Error: The output currency could not be found."); }
            double outputValue = outputCurrency.Value;

            return (true, (value / inputValue * outputValue).ToString("N2", CultureInfo.CreateSpecificCulture("en-US")), inputCurrency.Name, outputCurrency.Name);
        }

        public class Eur
        {
            [JsonProperty("1inch")]
            public double aed { get; set; }
            public double afn { get; set; }
            public double amd { get; set; }
            public double ang { get; set; }
            public double aoa { get; set; }
            public double ape { get; set; }
            public double apt { get; set; }
            public double ar { get; set; }
            public double arb { get; set; }
            public double ars { get; set; }
            public double atom { get; set; }
            public double ats { get; set; }
            public double aud { get; set; }
            public double avax { get; set; }
            public double awg { get; set; }
            public double axs { get; set; }
            public double azm { get; set; }
            public double azn { get; set; }
            public double bake { get; set; }
            public double bam { get; set; }
            public double bat { get; set; }
            public double bbd { get; set; }
            public double bch { get; set; }
            public double bdt { get; set; }
            public double bef { get; set; }
            public double bgn { get; set; }
            public double bhd { get; set; }
            public double bif { get; set; }
            public double bmd { get; set; }
            public double bnb { get; set; }
            public double bnd { get; set; }
            public double bob { get; set; }
            public double brl { get; set; }
            public double bsd { get; set; }
            public double bsv { get; set; }
            public double bsw { get; set; }
            public double btc { get; set; }
            public double btcb { get; set; }
            public double btg { get; set; }
            public double btn { get; set; }
            public double btt { get; set; }
            public double busd { get; set; }
            public double bwp { get; set; }
            public double byn { get; set; }
            public double byr { get; set; }
            public double bzd { get; set; }
            public double cad { get; set; }
            public double cake { get; set; }
            public double cdf { get; set; }
            public double celo { get; set; }
            public double cfx { get; set; }
            public double chf { get; set; }
            public double chz { get; set; }
            public double clp { get; set; }
            public double cnh { get; set; }
            public double cny { get; set; }
            public double comp { get; set; }
            public double cop { get; set; }
            public double crc { get; set; }
            public double cro { get; set; }
            public double crv { get; set; }
            public double cspr { get; set; }
            public double cuc { get; set; }
            public double cup { get; set; }
            public double cve { get; set; }
            public double cvx { get; set; }
            public double cyp { get; set; }
            public double czk { get; set; }
            public double dai { get; set; }
            public double dash { get; set; }
            public double dcr { get; set; }
            public double dem { get; set; }
            public double dfi { get; set; }
            public double djf { get; set; }
            public double dkk { get; set; }
            public double doge { get; set; }
            public double dop { get; set; }
            public double dot { get; set; }
            public double dydx { get; set; }
            public double dzd { get; set; }
            public double eek { get; set; }
            public double egld { get; set; }
            public double egp { get; set; }
            public double enj { get; set; }
            public double eos { get; set; }
            public double ern { get; set; }
            public double esp { get; set; }
            public double etb { get; set; }
            public double etc { get; set; }
            public double eth { get; set; }
            public int eur { get; set; }
            public double fei { get; set; }
            public double fil { get; set; }
            public double fim { get; set; }
            public double fjd { get; set; }
            public double fkp { get; set; }
            public double flow { get; set; }
            public double flr { get; set; }
            public double frax { get; set; }
            public double frf { get; set; }
            public double ftt { get; set; }
            public double fxs { get; set; }
            public double gala { get; set; }
            public double gbp { get; set; }
            public double gel { get; set; }
            public double ggp { get; set; }
            public double ghc { get; set; }
            public double ghs { get; set; }
            public double gip { get; set; }
            public double gmd { get; set; }
            public double gmx { get; set; }
            public double gnf { get; set; }
            public double gno { get; set; }
            public double grd { get; set; }
            public double grt { get; set; }
            public double gt { get; set; }
            public double gtq { get; set; }
            public double gusd { get; set; }
            public double gyd { get; set; }
            public double hbar { get; set; }
            public double hkd { get; set; }
            public double hnl { get; set; }
            public double hnt { get; set; }
            public double hot { get; set; }
            public double hrk { get; set; }
            public double ht { get; set; }
            public double htg { get; set; }
            public double huf { get; set; }
            public double icp { get; set; }
            public double idr { get; set; }
            public double iep { get; set; }
            public double ils { get; set; }
            public double imp { get; set; }
            public double imx { get; set; }
            public double inj { get; set; }
            public double inr { get; set; }
            public double iqd { get; set; }
            public double irr { get; set; }
            public double isk { get; set; }
            public double itl { get; set; }
            public double jep { get; set; }
            public double jmd { get; set; }
            public double jod { get; set; }
            public double jpy { get; set; }
            public double kas { get; set; }
            public double kava { get; set; }
            public double kcs { get; set; }
            public double kda { get; set; }
            public double kes { get; set; }
            public double kgs { get; set; }
            public double khr { get; set; }
            public double klay { get; set; }
            public double kmf { get; set; }
            public double knc { get; set; }
            public double kpw { get; set; }
            public double krw { get; set; }
            public double ksm { get; set; }
            public double kwd { get; set; }
            public double kyd { get; set; }
            public double kzt { get; set; }
            public double lak { get; set; }
            public double lbp { get; set; }
            public double ldo { get; set; }
            public double leo { get; set; }
            public double link { get; set; }
            public double lkr { get; set; }
            public double lrc { get; set; }
            public double lrd { get; set; }
            public double lsl { get; set; }
            public double ltc { get; set; }
            public double ltl { get; set; }
            public double luf { get; set; }
            public double luna { get; set; }
            public double lunc { get; set; }
            public double lvl { get; set; }
            public double lyd { get; set; }
            public double mad { get; set; }
            public double mana { get; set; }
            public double mbx { get; set; }
            public double mdl { get; set; }
            public double mga { get; set; }
            public double mgf { get; set; }
            public double mina { get; set; }
            public double mkd { get; set; }
            public double mkr { get; set; }
            public double mmk { get; set; }
            public double mnt { get; set; }
            public double mop { get; set; }
            public double mro { get; set; }
            public double mru { get; set; }
            public double mtl { get; set; }
            public double mur { get; set; }
            public double mvr { get; set; }
            public double mwk { get; set; }
            public double mxn { get; set; }
            public double mxv { get; set; }
            public double myr { get; set; }
            public double mzm { get; set; }
            public double mzn { get; set; }
            public double nad { get; set; }
            public double near { get; set; }
            public double neo { get; set; }
            public double nexo { get; set; }
            public double nft { get; set; }
            public double ngn { get; set; }
            public double nio { get; set; }
            public double nlg { get; set; }
            public double nok { get; set; }
            public double npr { get; set; }
            public double nzd { get; set; }
            public double okb { get; set; }
            public double omr { get; set; }
            public double one { get; set; }
            public double op { get; set; }
            public double ordi { get; set; }
            public double pab { get; set; }
            public double paxg { get; set; }
            public double pen { get; set; }
            public double pepe { get; set; }
            public double pgk { get; set; }
            public double php { get; set; }
            public double pkr { get; set; }
            public double pln { get; set; }
            public double pte { get; set; }
            public double pyg { get; set; }
            public double qar { get; set; }
            public double qnt { get; set; }
            public double qtum { get; set; }
            public double rol { get; set; }
            public double ron { get; set; }
            public double rpl { get; set; }
            public double rsd { get; set; }
            public double rub { get; set; }
            public double rune { get; set; }
            public double rvn { get; set; }
            public double rwf { get; set; }
            public double sand { get; set; }
            public double sar { get; set; }
            public double sbd { get; set; }
            public double scr { get; set; }
            public double sdd { get; set; }
            public double sdg { get; set; }
            public double sek { get; set; }
            public double sgd { get; set; }
            public double shib { get; set; }
            public double shp { get; set; }
            public double sit { get; set; }
            public double skk { get; set; }
            public double sle { get; set; }
            public double sll { get; set; }
            public double snx { get; set; }
            public double sol { get; set; }
            public double sos { get; set; }
            public double spl { get; set; }
            public double srd { get; set; }
            public double srg { get; set; }
            public double std { get; set; }
            public double stn { get; set; }
            public double stx { get; set; }
            public double sui { get; set; }
            public double svc { get; set; }
            public double syp { get; set; }
            public double szl { get; set; }
            public double thb { get; set; }
            public double theta { get; set; }
            public double tjs { get; set; }
            public double tmm { get; set; }
            public double tmt { get; set; }
            public double tnd { get; set; }
            public double ton { get; set; }
            public double top { get; set; }
            public double trl { get; set; }
            public double trx { get; set; }
            public double @try { get; set; }
            public double ttd { get; set; }
            public double tusd { get; set; }
            public double tvd { get; set; }
            public double twd { get; set; }
            public double twt { get; set; }
            public double tzs { get; set; }
            public double uah { get; set; }
            public double ugx { get; set; }
            public double uni { get; set; }
            public double usd { get; set; }
            public double usdc { get; set; }
            public double usdd { get; set; }
            public double usdp { get; set; }
            public double usdt { get; set; }
            public double uyu { get; set; }
            public double uzs { get; set; }
            public double val { get; set; }
            public double veb { get; set; }
            public double ved { get; set; }
            public double vef { get; set; }
            public double ves { get; set; }
            public double vet { get; set; }
            public double vnd { get; set; }
            public double vuv { get; set; }
            public double waves { get; set; }
            public double wemix { get; set; }
            public double woo { get; set; }
            public double wst { get; set; }
            public double xaf { get; set; }
            public double xag { get; set; }
            public double xau { get; set; }
            public double xaut { get; set; }
            public double xbt { get; set; }
            public double xcd { get; set; }
            public double xcg { get; set; }
            public double xch { get; set; }
            public double xdc { get; set; }
            public double xdr { get; set; }
            public double xec { get; set; }
            public double xem { get; set; }
            public double xlm { get; set; }
            public double xmr { get; set; }
            public double xof { get; set; }
            public double xpd { get; set; }
            public double xpf { get; set; }
            public double xpt { get; set; }
            public double xrp { get; set; }
            public double xtz { get; set; }
            public double yer { get; set; }
            public double zar { get; set; }
            public double zec { get; set; }
            public double zmk { get; set; }
            public double zmw { get; set; }
            public double zwd { get; set; }
            public double zwg { get; set; }
            public double zwl { get; set; }
        }

        public class EurRates
        {
            public string date { get; set; }
            public Eur eur { get; set; }
        }

        private void Read()
        {
            EurRates eurRates = new();

            using (FileStream readRates = File.OpenRead(Program.currencyPath))
            {
                eurRates = System.Text.Json.JsonSerializer.Deserialize<EurRates>(readRates);
            }

            LastUpdated = DateTime.ParseExact($"{eurRates.date}-06-00", "yyyy-MM-dd-HH-mm", CultureInfo.InvariantCulture);
            //The API updates at an irregular time between ~3:00–9:00. The 6:00 reference time is meant to average it out.

            Currencies = [
                new Currency("AED", "United Arab Emirates dirham", eurRates.eur.aed),
                new Currency("AFN", "Afghan afghani", eurRates.eur.afn),
                new Currency("AMD", "Armenian dram", eurRates.eur.amd),
                new Currency("ANG", "Netherlands Antillean guilder", eurRates.eur.ang),
                new Currency("ARS", "Argentine Peso", eurRates.eur.ars),
                new Currency("AUD", "Australian dollarydoo", eurRates.eur.aud),
                new Currency("AWG", "Aruban florin", eurRates.eur.awg),
                new Currency("AZN", "Azerbaijani manat", eurRates.eur.azn),
                new Currency("BAM", "Bosnia-Herzegovina convertible marka", eurRates.eur.bam),
                new Currency("BBD", "Bajan dollar", eurRates.eur.bbd),
                new Currency("BDT", "Bangladeshi taka", eurRates.eur.bdt),
                new Currency("BGN", "Bulgarian lev", eurRates.eur.bgn),
                new Currency("BHD", "Bahraini dinar", eurRates.eur.bhd),
                new Currency("BIF", "Burundian franc", eurRates.eur.bif),
                new Currency("BMD", "Bermuda dollar", eurRates.eur.bmd),
                new Currency("BND", "Brunei dollar", eurRates.eur.bnd),
                new Currency("BOB", "Bolivian boliviano", eurRates.eur.bob),
                new Currency("BRL", "Brazilian real", eurRates.eur.brl),
                new Currency("BSD", "Bahamian dollar", eurRates.eur.bsd),
                new Currency("BTN", "Bhutanese ngultrum", eurRates.eur.btn),
                new Currency("BWP", "Botswana pula", eurRates.eur.bwp),
                new Currency("BYN", "Belarusian ruble", eurRates.eur.byn),
                new Currency("BZD", "Belize dollar", eurRates.eur.bzd),
                new Currency("CAD", "Canadian dollar", eurRates.eur.cad),
                new Currency("CDF", "Congolese franc", eurRates.eur.cdf),
                new Currency("CHF", "Swiss franc", eurRates.eur.chf),
                new Currency("CLP", "Chilean peso", eurRates.eur.clp),
                new Currency("CNH", "Chinese yuan (onshore)", eurRates.eur.cnh),
                new Currency("CNY", "Chinese yuan (offshore)", eurRates.eur.cny),
                new Currency("COP", "Colombian peso", eurRates.eur.cop),
                new Currency("CRC", "Costa Rican colón", eurRates.eur.crc),
                new Currency("CUC", "Cuban peso", eurRates.eur.cuc),
                new Currency("CUP", "Cuban peso", eurRates.eur.cup),
                new Currency("CVE", "Cape Verdean escudo", eurRates.eur.cve),
                new Currency("CZK", "Czech koruna", eurRates.eur.czk),
                new Currency("DJF", "Djiboutian franc", eurRates.eur.djf),
                new Currency("DKK", "Danish krone", eurRates.eur.dkk),
                new Currency("DOP", "Dominican peso", eurRates.eur.dop),
                new Currency("DZD", "Algerian dinar", eurRates.eur.dzd),
                new Currency("EGP", "Egyptian pound", eurRates.eur.egp),
                new Currency("ERN", "Eritrean nakfa", eurRates.eur.ern),
                new Currency("ESP", "Spanish peseta", eurRates.eur.esp),
                new Currency("ETB", "Ethiopian birr", eurRates.eur.etb),
                new Currency("EUR", "euro", eurRates.eur.eur),
                new Currency("FJD", "Fijian dollar", eurRates.eur.fjd),
                new Currency("FKP", "Falkland Islands pound", eurRates.eur.fkp),
                new Currency("FRF", "French franc", eurRates.eur.frf),
                new Currency("GBP", "Pound sterling", eurRates.eur.gbp),
                new Currency("GEL", "Georgian lari", eurRates.eur.gel),
                new Currency("GGP", "Guernsey pound", eurRates.eur.ggp),
                new Currency("GHS", "Ghanaian cedi", eurRates.eur.ghs),
                new Currency("GIP", "Gibraltar pound", eurRates.eur.gip),
                new Currency("GMD", "Gambian dalasi", eurRates.eur.gmd),
                new Currency("GNF", "Guinean franc", eurRates.eur.gnf),
                new Currency("GTQ", "Guatemalan quetzal", eurRates.eur.gtq),
                new Currency("GYD", "Guyanese dollar", eurRates.eur.gyd),
                new Currency("HKD", "Hong Kong dollar", eurRates.eur.hkd),
                new Currency("HNL", "Honduran lempira", eurRates.eur.hnl),
                new Currency("HTG", "Haitian gourde", eurRates.eur.htg),
                new Currency("HUF", "Hungarian forint", eurRates.eur.huf),
                new Currency("IDR", "Indonesian rupiah", eurRates.eur.idr),
                new Currency("ILS", "new Israeli shekel", eurRates.eur.ils),
                new Currency("IMP", "Manx pound", eurRates.eur.imp),
                new Currency("INR", "Indian rupee", eurRates.eur.inr),
                new Currency("IQD", "Iraqi dinar", eurRates.eur.iqd),
                new Currency("IRR", "Iranian rial", eurRates.eur.irr),
                new Currency("ISK", "Icelandic króna", eurRates.eur.isk),
                new Currency("JEP", "Jersey pound", eurRates.eur.jep),
                new Currency("JMD", "Jamaican dollar", eurRates.eur.jmd),
                new Currency("JOD", "Jordanian dinar", eurRates.eur.jod),
                new Currency("JPY", "Japanese yen", eurRates.eur.jpy),
                new Currency("KES", "Kenyan shilling", eurRates.eur.kes),
                new Currency("KGS", "Kyrgystani som", eurRates.eur.kgs),
                new Currency("KHR", "Cambodian riel", eurRates.eur.khr),
                new Currency("KMF", "Comorian franc", eurRates.eur.kmf),
                new Currency("KPW", "North Korean won", eurRates.eur.kpw),
                new Currency("KRW", "South Korean won", eurRates.eur.krw),
                new Currency("KWD", "Kuwaiti dinar", eurRates.eur.kwd),
                new Currency("KYD", "Cayman Islands dollar", eurRates.eur.kyd),
                new Currency("KZT", "Kazakhstani tenge", eurRates.eur.kzt),
                new Currency("LAK", "Laotian kip", eurRates.eur.lak),
                new Currency("LBP", "Lebanese pound", eurRates.eur.lbp),
                new Currency("LKR", "Sri Lankan rupee", eurRates.eur.lkr),
                new Currency("LRD", "Liberian dollar", eurRates.eur.lrd),
                new Currency("LSL", "Lesotho loti", eurRates.eur.lsl),
                new Currency("LTL", "Lithuanian litas", eurRates.eur.ltl),
                new Currency("LYD", "Libyan dinar", eurRates.eur.lyd),
                new Currency("MAD", "Moroccan dirham", eurRates.eur.mad),
                new Currency("MDL", "Moldovan leu", eurRates.eur.mdl),
                new Currency("MGA", "Malagasy ariary", eurRates.eur.mga),
                new Currency("MKD", "Macedonian denar", eurRates.eur.mkd),
                new Currency("MMK", "Myanmar kyat", eurRates.eur.mmk),
                new Currency("MNT", "Mongolian tugrik", eurRates.eur.mnt),
                new Currency("MOP", "Macanese pataca", eurRates.eur.mop),
                new Currency("MRU", "Mauritanian ouguiya", eurRates.eur.mru),
                new Currency("MUR", "Mauritian rupee", eurRates.eur.mur),
                new Currency("MVR", "Maldivian rufiyaa", eurRates.eur.mvr),
                new Currency("MWK", "Malawian kwacha", eurRates.eur.mwk),
                new Currency("MXN", "Mexican peso", eurRates.eur.mxn),
                new Currency("MXV", "Mexican unidad de inversión", eurRates.eur.mxv),
                new Currency("MYR", "Malaysian ringgit", eurRates.eur.myr),
                new Currency("MZN", "Mozambican metical", eurRates.eur.mzn),
                new Currency("NAD", "Namibian dollar", eurRates.eur.nad),
                new Currency("NGN", "Nigerian naira", eurRates.eur.ngn),
                new Currency("NIO", "Nicaraguan córdoba", eurRates.eur.nio),
                new Currency("NOK", "Norwegian krone", eurRates.eur.nok),
                new Currency("NPR", "Nepalese rupee", eurRates.eur.npr),
                new Currency("NZD", "New Zealand dollar", eurRates.eur.nzd),
                new Currency("OMR", "Omani rial", eurRates.eur.omr),
                new Currency("PAB", "Panamanian balboa", eurRates.eur.pab),
                new Currency("PEN", "sol", eurRates.eur.pen),
                new Currency("PGK", "Papua New Guinean kina", eurRates.eur.pgk),
                new Currency("PHP", "Philippine peso", eurRates.eur.php),
                new Currency("PKR", "Pakistani rupee", eurRates.eur.pkr),
                new Currency("PLN", "Polish złoty", eurRates.eur.pln),
                new Currency("PTE", "Portuguese escudo", eurRates.eur.pte),
                new Currency("PYG", "Paraguayan guarani", eurRates.eur.pyg),
                new Currency("QAR", "Qatari riyal", eurRates.eur.qar),
                new Currency("RON", "Romanian leu", eurRates.eur.ron),
                new Currency("RSD", "Serbian dinar", eurRates.eur.rsd),
                new Currency("RUB", "Russian ruble", eurRates.eur.rub),
                new Currency("RWF", "Rwandan franc", eurRates.eur.rwf),
                new Currency("SAR", "Saudi riyal", eurRates.eur.sar),
                new Currency("SBD", "Solomon Islands dollar", eurRates.eur.sbd),
                new Currency("SCR", "Seychellois rupee", eurRates.eur.scr),
                new Currency("SDG", "Sudanese pound", eurRates.eur.sdg),
                new Currency("SEK", "Swedish krona", eurRates.eur.sek),
                new Currency("SGD", "Singapore dollar", eurRates.eur.sgd),
                new Currency("SHP", "Saint Helena pound", eurRates.eur.shp),
                new Currency("SLL", "Sierra Leonean leone", eurRates.eur.sll),
                new Currency("SOS", "Somali shilling", eurRates.eur.sos),
                new Currency("SPL", "Seborgan luigino", eurRates.eur.spl),
                new Currency("SRD", "Suriname dollar", eurRates.eur.srd),
                new Currency("STN", "São Tomé and Príncipe dobra", eurRates.eur.stn),
                new Currency("SYP", "Syrian pound", eurRates.eur.syp),
                new Currency("SZL", "Swazi lilangeni", eurRates.eur.szl),
                new Currency("THB", "Thai baht", eurRates.eur.thb),
                new Currency("TJS", "Tajikistani somoni", eurRates.eur.tjs),
                new Currency("TMT", "Turkmenistani manat", eurRates.eur.tmt),
                new Currency("TND", "Tunisian dinar", eurRates.eur.tnd),
                new Currency("TRL", "Turkish lira", eurRates.eur.trl),
                new Currency("TRY", "Turkish lira", eurRates.eur.@try),
                new Currency("TTD", "Trinidad & Tobago dollar", eurRates.eur.ttd),
                new Currency("TVD", "Tuvaluan dollar", eurRates.eur.tvd),
                new Currency("TWD", "New Taiwan dollar", eurRates.eur.twd),
                new Currency("TZS", "Tanzanian shilling", eurRates.eur.tzs),
                new Currency("UAH", "Ukrainian hryvnia", eurRates.eur.uah),
                new Currency("UGX", "Ugandan shilling", eurRates.eur.ugx),
                new Currency("USD", "United States dollar", eurRates.eur.usd),
                new Currency("UYU", "Uruguayan peso", eurRates.eur.uyu),
                new Currency("UZS", "Uzbekistani som", eurRates.eur.uzs),
                new Currency("VES", "Venezuelan bolívar", eurRates.eur.ves),
                new Currency("VND", "Vietnamese dong", eurRates.eur.vnd),
                new Currency("VUV", "Vanuatu vatu", eurRates.eur.vuv),
                new Currency("WST", "Samoan tālā", eurRates.eur.wst),
                new Currency("XAF", "Central African CFA franc", eurRates.eur.xaf),
                new Currency("XAG", "silver ounce", eurRates.eur.xag),
                new Currency("XAU", "gold ounce", eurRates.eur.xau),
                new Currency("XCD", "East Caribbean dollar", eurRates.eur.xcd),
                new Currency("XCG", "Caribbean guilder", eurRates.eur.xcg),
                new Currency("XDR", "special drawing rights", eurRates.eur.xdr),
                new Currency("XOF", "West African CFA franc", eurRates.eur.xof),
                new Currency("XPD", "palladium ounce", eurRates.eur.xpd),
                new Currency("XPF", "CFP franc", eurRates.eur.xpf),
                new Currency("XPT", "platinum ounce", eurRates.eur.xpt),
                new Currency("YER", "Yemeni rial", eurRates.eur.yer),
                new Currency("ZAR", "South African rand", eurRates.eur.zar),
                new Currency("ZMW", "Zambian kwacha", eurRates.eur.zmw),
                new Currency("ZWG", "Zimbabwe gold", eurRates.eur.zwg)
            ];
        }
    }
}
