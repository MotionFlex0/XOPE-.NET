namespace SDK
{
    public interface IScript
    {
        //Guid Id { get; set; }

        void OnInit();
        void OnTick();
        void OnExit();
    }
}
