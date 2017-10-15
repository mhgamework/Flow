using DirectX11;
using UnityEngine;

namespace Assets.Gameplay
{
    public class LocalPlayerSingleton : Singleton<LocalPlayerSingleton>
    {
        public InventoryScript Inventory;
        /// <summary>
        /// Magic / cubic meter / second (maybe changed to square meter)
        /// </summary>
        public float TerrainToolsMagicConsumptionFactor = 1f;
        public string TerrainToolDepositMagic = "Earth";
        public string TerrainToolWithdrawMagic = "Fire";
        public string TerrainToolSmoothMagic = "Water";
        public string TerrainToolFlattenMagic = "Water";
        public string JetpackMagic = "Air";
        public float JetpackMagicConsumptionRate = 1f;


        
        public bool TryUseDepositMagic(float depositRadius)
        {
            return tryConsumeMagicForTerrainTool(TerrainToolDepositMagic, depositRadius);
        }

        public bool TryUseWithdrawMagic(float depositRadius)
        {
            return tryConsumeMagicForTerrainTool(TerrainToolWithdrawMagic, depositRadius);
        }
        public bool TryUseSmoothMagic(float depositRadius)
        {
            return tryConsumeMagicForTerrainTool(TerrainToolSmoothMagic, depositRadius);
        }
        public bool TryUseFlattenMagic(float depositRadius)
        {
            return tryConsumeMagicForTerrainTool(TerrainToolFlattenMagic, depositRadius);
        }

        private bool tryConsumeMagicForTerrainTool(string magicType, float depositRadius)
        {
            var consumption = calculateTerrainToolConsumption(depositRadius);
            return tryTakeMagic(magicType, consumption);
        }

        private bool tryTakeMagic(string magicType, float consumption)
        {
            return Inventory.Take(magicType, consumption) > consumption - 0.001;
        }

        private float calculateTerrainToolConsumption(float radius)
        {
            //var volume = 4 / 3f * Mathf.PI * absRadius * absRadius * absRadius;
            var surface = Mathf.PI * radius * radius;
            var consumptionSpeed = TerrainToolsMagicConsumptionFactor * surface;

            var consumption = consumptionSpeed * Time.deltaTime;
            return consumption;
        }

        public bool TryUseFlyMagic()
        {
            var consumption = JetpackMagicConsumptionRate * Time.deltaTime;
            return tryTakeMagic(JetpackMagic, consumption);
        }
    }
}