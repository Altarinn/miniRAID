namespace miniRAID.TurnSchedule
{
    public struct Timestamp
    {
        public string currentPhase;
        public int currentTurnID;
        // Turnslices inside a turn is highly dynamic and tracking its serial ID is error prone.
        // public int currentTurnSliceID;
        
        public Timestamp(int turnID)
        {
            currentTurnID = turnID;
            currentPhase = "default";
            // currentTurnSliceID = -1;
        }
    }
}