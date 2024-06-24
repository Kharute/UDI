using UnityEngine.TextCore.Text;

public static class PlayerInfoExtentions
{
    /// <summary>
    /// 좌상단 Player 정보에 관한 Extentions.
    /// </summary>
    /// <param name="vm"></param>
    #region PlayerInfo
    public static void RefreshViewModel_PlayerInfo(this PlayerInfoViewModel vm)
    {
        DataBaseManager.Inst.RefreshPlayerInfo(vm.OnRefreshViewModel_PlayerInfo);
    }
    public static void OnRefreshViewModel_PlayerInfo(this PlayerInfoViewModel vm, UserDetails userDetails)
    {
        vm.Name = userDetails.NICKNAME;
        vm.Level = userDetails.LEVEL;
        vm.Exp = userDetails.EXPERIENCE;

    }
    public static void RegisterEventsOnEnable_PlayerInfo(this PlayerInfoViewModel vm, bool isEventEnable)
    {
        DataBaseManager.Inst.RegisterExpUpCallback(vm.OnResponseExpChangedEvent, isEventEnable);
        DataBaseManager.Inst.RegisterLevelUpCallback(vm.OnResponseLevelChangedEvent, isEventEnable);
    }

    public static void OnResponseExpChangedEvent(this PlayerInfoViewModel vm, int exp)
    {
        vm.Exp = exp;
    }

    public static void OnResponseLevelChangedEvent(this PlayerInfoViewModel vm, int level)
    {
        vm.Level = level;
    }

    public static void RegisterEventsOnEnable(this PlayerInfoViewModel vm, bool isEventEnable)
    {
        DataBaseManager.Inst.RegisterExpUpCallback(vm.OnResponseExpChangedEvent, isEventEnable);
        DataBaseManager.Inst.RegisterLevelUpCallback(vm.OnResponseLevelChangedEvent, isEventEnable);
    }
    #endregion

    /// <summary>
    /// 우상단 Goods 정보에 관한 Extentions.
    /// </summary>
    /// <param name="vm"></param>
    #region Goods

    public static void RefreshViewModel_Goods(this GoodsViewModel vm)
    {
        DataBaseManager.Inst.RefreshPlayerInfo(vm.OnRefreshViewModel_PlayerInfo);
    }
    public static void OnRefreshViewModel_PlayerInfo(this GoodsViewModel vm, UserDetails userDetails)
    {
        vm.Gold = userDetails.GOLD;
        vm.Jewel = userDetails.JEWEL;
    }

    public static void RegisterEventsOnEnable_Goods(this GoodsViewModel vm, bool isEventEnable)
    {
        DataBaseManager.Inst.RegisterGoldChangedCallback(vm.OnResponseGoldChangedEvent, isEventEnable);
        DataBaseManager.Inst.RegisterJewelChangedCallback(vm.OnResponseJewelChangedEvent, isEventEnable);
    }

    public static void OnResponseGoldChangedEvent(this GoodsViewModel vm, int gold)
    {
        vm.Gold = gold;
    }

    public static void OnResponseJewelChangedEvent(this GoodsViewModel vm, int jewel)
    {
        vm.Jewel = jewel;
    }

    #endregion

    #region PlayerDetails
    public static Levels GetPlayerDetailData(this GameDataManager manager, int level)
    {
        var levelInfoList = manager.LevelInfoList;
        if (levelInfoList.Count == 0
            || levelInfoList.ContainsKey(level) == false)
        {
            return null;
        }

        return levelInfoList[level];
    }

    #endregion

}
