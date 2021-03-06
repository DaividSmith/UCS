﻿/*
 * Program : Ultrapowa Clash Server
 * Description : A C# Writted 'Clash of Clans' Server Emulator !
 *
 * Authors:  Jean-Baptiste Martin <Ultrapowa at Ultrapowa.com>,
 *           And the Official Ultrapowa Developement Team
 *
 * Copyright (c) 2016  UltraPowa
 * All Rights Reserved.
 */

using Newtonsoft.Json.Linq;
using UCS.Core;
using UCS.Files.Logic;
using UCS.Helpers;

namespace UCS.Logic
{
    internal class UnitUpgradeComponent : Component
    {
        //a1 + 12
        //a1 + 16
        //a1 + 20 -- Listener?

        #region Public Constructors

        public UnitUpgradeComponent(GameObject go) : base(go)
        {
            m_vTimer = null;
            m_vCurrentlyUpgradedUnit = null;
        }

        #endregion Public Constructors

        #region Public Properties

        public override int Type
        {
            get { return 9; }
        }

        #endregion Public Properties

        #region Private Fields

        private CombatItemData m_vCurrentlyUpgradedUnit;
        private Timer m_vTimer;

        #endregion Private Fields

        #region Public Methods

        public bool CanStartUpgrading(CombatItemData cid)
        {
            var result = false;
            if (m_vCurrentlyUpgradedUnit == null)
            {
                var b = (Building) GetParent();
                var ca = GetParent().GetLevel().GetHomeOwnerAvatar();
                var cm = GetParent().GetLevel().GetComponentManager();
                int maxProductionBuildingLevel;
                if (cid.GetCombatItemType() == 1)
                    maxProductionBuildingLevel = cm.GetMaxSpellForgeLevel();
                else
                    maxProductionBuildingLevel = cm.GetMaxBarrackLevel();
                if (ca.GetUnitUpgradeLevel(cid) < cid.GetUpgradeLevelCount() - 1)
                {
                    if (maxProductionBuildingLevel >= cid.GetRequiredProductionHouseLevel() - 1)
                    {
                        result = b.GetUpgradeLevel() >=
                                 cid.GetRequiredLaboratoryLevel(ca.GetUnitUpgradeLevel(cid) + 1) - 1;
                    }
                }
            }
            return result;
        }

        public void FinishUpgrading()
        {
            if (m_vCurrentlyUpgradedUnit != null)
            {
                var ca = GetParent().GetLevel().GetHomeOwnerAvatar();
                var level = ca.GetUnitUpgradeLevel(m_vCurrentlyUpgradedUnit);
                ca.SetUnitUpgradeLevel(m_vCurrentlyUpgradedUnit, level + 1);
            }
            m_vTimer = null;
            m_vCurrentlyUpgradedUnit = null;
        }

        public CombatItemData GetCurrentlyUpgradedUnit()
        {
            return m_vCurrentlyUpgradedUnit;
        }

        public int GetRemainingSeconds()
        {
            var result = 0;
            if (m_vTimer != null)
            {
                result = m_vTimer.GetRemainingSeconds(GetParent().GetLevel().GetTime());
            }
            return result;
        }

        public int GetTotalSeconds()
        {
            var result = 0;
            if (m_vCurrentlyUpgradedUnit != null)
            {
                var ca = GetParent().GetLevel().GetHomeOwnerAvatar();
                var level = ca.GetUnitUpgradeLevel(m_vCurrentlyUpgradedUnit);
                result = m_vCurrentlyUpgradedUnit.GetUpgradeTime(level);
            }
            return result;
        }

        public override void Load(JObject jsonObject)
        {
            var unitUpgradeObject = (JObject) jsonObject["unit_upg"];
            if (unitUpgradeObject != null)
            {
                m_vTimer = new Timer();
                var remainingTime = unitUpgradeObject["t"].ToObject<int>();
                m_vTimer.StartTimer(remainingTime, GetParent().GetLevel().GetTime());

                var id = unitUpgradeObject["id"].ToObject<int>();
                m_vCurrentlyUpgradedUnit = (CombatItemData) ObjectManager.DataTables.GetDataById(id);
            }
        }

        public override JObject Save(JObject jsonObject)
        {
            //{"data":1000007,"lvl":7,"x":4,"y":4,"unit_upg":{"unit_type":0,"t":591612,"id":4000001},"l1x":32,"l1y":32}

            if (m_vCurrentlyUpgradedUnit != null)
            {
                var unitUpgradeObject = new JObject();

                unitUpgradeObject.Add("unit_type", m_vCurrentlyUpgradedUnit.GetCombatItemType());
                unitUpgradeObject.Add("t", m_vTimer.GetRemainingSeconds(GetParent().GetLevel().GetTime()));
                unitUpgradeObject.Add("id", m_vCurrentlyUpgradedUnit.GetGlobalID());
                jsonObject.Add("unit_upg", unitUpgradeObject);
            }
            return jsonObject;
        }

        public void SpeedUp()
        {
            if (m_vCurrentlyUpgradedUnit != null)
            {
                var remainingSeconds = 0;
                if (m_vTimer != null)
                {
                    remainingSeconds = m_vTimer.GetRemainingSeconds(GetParent().GetLevel().GetTime());
                }
                var cost = GamePlayUtil.GetSpeedUpCost(remainingSeconds);
                var ca = GetParent().GetLevel().GetPlayerAvatar();
                if (ca.HasEnoughDiamonds(cost))
                {
                    ca.UseDiamonds(cost);
                    FinishUpgrading();
                }
            }
        }

        public void StartUpgrading(CombatItemData cid)
        {
            if (CanStartUpgrading(cid))
            {
                m_vCurrentlyUpgradedUnit = cid;
                m_vTimer = new Timer();
                m_vTimer.StartTimer(GetTotalSeconds(), GetParent().GetLevel().GetTime());
            }
        }

        public override void Tick()
        {
            if (m_vTimer != null)
            {
                if (m_vTimer.GetRemainingSeconds(GetParent().GetLevel().GetTime()) <= 0)
                {
                    FinishUpgrading();
                }
            }
        }

        #endregion Public Methods
    }
}