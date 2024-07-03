using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace ViewModel.Extentions
{
    public static class GameDataModelExtentions
    {
        public static Skill GetSkillData(this GameDataManager manager, string dataClassName)
        {
            var loadedSkillList = manager.SkillInfoList;
            if (loadedSkillList.Count == 0
                || loadedSkillList.ContainsKey(dataClassName) == false)
            {
                return null;
            }
            return loadedSkillList[dataClassName];
        }

        public static Item GetWeaponData(this GameDataManager manager, int weaponId)
        {
            /*var loadedItemList = manager.ItemInfoList;
            
            if (loadedItemList.Count == 0 || loadedItemList.ContainsKey(weaponName) == false)
            {
                return null;
            }*/
            return null;
                //loadedItemList[weaponName];
        }
    }
}

