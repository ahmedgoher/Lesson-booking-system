namespace Al_Muzayyen.Viewmodel
{
    public class GroupActionVM
    {
        public int Id { get; set; }
        public string Group_Name { get; set; }
        public int PlaceId { get; set; }
        public int ClassId { get; set; }
        public int Number_Of_day { get; set; }
        public List<SlotTimeVM> SlotTimes { get; set; }
    }
}
