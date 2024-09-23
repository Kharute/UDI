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
        vm.Name = userDetails.nickname;
        vm.Level = userDetails.level;
        vm.Exp = userDetails.experience;
    }
    public static void OnResponseNameChangedEvent(this PlayerInfoViewModel vm, string name)
    {
        vm.Name = name;
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
        DataBaseManager.Inst.RegisterLoginCallback(vm.OnResponseNameChangedEvent, isEventEnable);
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
        vm.Gold = userDetails.gold;
        vm.Jewel = userDetails.jewel;
    }

    public static void RegisterEventsOnEnable_Goods(this GoodsViewModel vm, bool isEventEnable)
    {
        DataBaseManager.Inst.Register_GoodsChangedCallback(vm.OnResponse_GoodsChangedEvent, UserGoodsType.gold, isEventEnable);
        DataBaseManager.Inst.Register_GoodsChangedCallback(vm.OnResponse_GoodsChangedEvent, UserGoodsType.jewel, isEventEnable);
        DataBaseManager.Inst.Register_GoodsChangedCallback(vm.OnResponse_GoodsChangedEvent, UserGoodsType.ticket_weapon, isEventEnable);
        DataBaseManager.Inst.Register_GoodsChangedCallback(vm.OnResponse_GoodsChangedEvent, UserGoodsType.ticket_armor, isEventEnable);
    }

    public static void OnResponse_GoodsChangedEvent(this GoodsViewModel vm, UserGoodsType type, int value)
    {
        switch(type)
        {
            case UserGoodsType.gold: 
                vm.Gold = value; break;
            case UserGoodsType.jewel: 
                vm.Jewel = value; break;
            case UserGoodsType.ticket_weapon: 
                vm.Ticket_Weapon = value; break;
            case UserGoodsType.ticket_armor: 
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
