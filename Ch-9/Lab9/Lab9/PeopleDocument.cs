namespace Lab9
{
    //定義people集合內的文件結構，並命名為PeopleDocument
    class PeopleDocument
    {
        public string _id { get; set; }
        public int statistic_yyymm { get; set; }
        public string site_id { get; set; }
        public string village { get; set; }
        public string district_code { get; set; }
        public int birth_total { get; set; }
        public int birth_total_m { get; set; }
        public int birth_total_f { get; set; }
        public int death_total { get; set; }
        public int death_m { get; set; }
        public int death_f { get; set; }
        public int marry_pair { get; set; }
        public int divorce_pair { get; set; }
    }
}