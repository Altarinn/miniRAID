namespace miniRAID.Backend.Numericals.Impl
{
    public static class BasicNumericals
    {
        public static void Fill(NumericalDatabase<string> db)
        {
            // #########################################
            // ## Starting points of numerical design ##
            // #########################################
            
            // 战斗最大人数
            db.StoreStat("global.maxPlayers", Consts.maxPlayers);
            
            // 基础每回合行动数
            db.StoreStat("global.actionsPerTurn", Consts.basePlayerPerTurn);
            
            // 每回合基础AP回复
            db.StoreStat("global.baseAPRegenTurn", Consts.baseAPRegenTurn);
            
            // 同水平（等级、平均物品等级）下，单个角色的治疗与伤害比值
            db.StoreStat("global.healDamageRatio", );
            
            // ## 玩家属性
            // 每等级基础能力值成长率的平均值
            db.StoreStat("global.baseStatAverageGrowth", Consts.baseStatAverageGrowth);
            
            // 一级时基础能力值平均值
            db.StoreStat("global.baseStatAverage", Consts.baseStatBaseLv1);
            
            // ## 装备属性
            
            
        }
    }
}