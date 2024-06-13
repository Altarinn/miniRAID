namespace miniRAID.TurnSchedule
{
    public struct Timestamp
    {
        public int currentTurnID;
        public int currentTurnSliceID;
        
        public Timestamp(int turnID)
        {
            currentTurnID = turnID;
            currentTurnSliceID = -1;
        }
    }
}