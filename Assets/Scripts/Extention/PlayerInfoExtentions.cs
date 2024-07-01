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
        //DataBaseManager.Inst.RegisterNameChangeCallback(vm.OnResponseNameChangedEvent, isEventEnable);
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
    public static void OnRefreshViewModel_PlayerInfo(this GoodsViewModel vm, UserGoods userDetails)
    {
        vm.Gold = userDetails.GOLD;
        vm.Jewel = userDetails.JEWEL;
    }

    public static void RegisterEventsOnEnable_Goods(this GoodsViewModel vm, bool isEventEnable)
    {
        DataBaseManager.Inst.Register_GoodsChangedCallback(vm.OnResponse_GoodsChangedEvent, UserGoodsType.GOLD, isEventEnable);
        DataBaseManager.Inst.Register_GoodsChangedCallback(vm.OnResponse_GoodsChangedEvent, UserGoodsType.JEWEL, isEventEnable);
        DataBaseManager.Inst.Register_GoodsChangedCallback(vm.OnResponse_GoodsChangedEvent, UserGoodsType.TICKET_WEAPON, isEventEnable);
        DataBaseManager.Inst.Register_GoodsChangedCallback(vm.OnResponse_GoodsChangedEvent, UserGoodsType.TICKET_ARMOR, isEventEnable);
    }

    public static void OnResponse_GoodsChangedEvent(this GoodsViewModel vm, UserGoodsType type, int value)
    {
        switch(type)
        {
            case UserGoodsType.GOLD: 
                vm.Gold = value; break;
            case UserGoodsType.JEWEL: 
                vm.Jewel = value; break;
            case UserGoodsType.TICKET_WEAPON: 
                vm.Ticket_Weapon = value; break;
            case UserGoodsType.TICKET_ARMOR: 
                vm.Ticket_Armor = value; break;
        }
    }

    #endregion

    #region PlayerDetails
    public static Levels GetPlayerDetailData(this GameDataManager manager, int level)
    {
        var levelInfoList = manager.LevelInfoList;

        if (levelInfoList == null)
        {
            return null;
        }
        else if(levelInfoList.Count == 0 || levelInfoList.ContainsKey(level) == false)
        {
            return null;
        }

        return levelInfoList[level];
    }

    #endregion

}
