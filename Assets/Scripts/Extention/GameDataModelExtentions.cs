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

        public static Weapon GetWeaponData(this GameDataManager manager, int weaponID)
        {
            var weaponList = manager.WeaponList;
            
            if (weaponList.Count == 0)
            {
                return null;
            }
            return weaponList[weaponID];
        }
    }
}

