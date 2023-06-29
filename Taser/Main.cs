using System.Collections;
using System.Collections.Generic;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace Taser
{
    public class Main: RocketPlugin<Config>
    {
        private List<Coroutine> _tasingCoroutines;
        
        protected override void Load()
        {
            _tasingCoroutines = new List<Coroutine>();
            
            DamageTool.damagePlayerRequested += DamageToolOnDamagePlayerRequested;
            
            Logger.Log("Kamiluk || Taser plugin has been loaded.");
        }

        private IEnumerator TasingEffects(Player player, float time)
        {
            if (Configuration.Instance.MakePlayerExitVehicle)
            {
                VehicleManager.forceRemovePlayer(player.channel.owner.playerID.steamID);
            }
            
            if (Configuration.Instance.MakePlayerStop)
            {
                player.movement.sendPluginSpeedMultiplier(0);
                player.movement.sendPluginJumpMultiplier(0);
            }

            while (true)
            {
                yield return new WaitForSeconds(0.1f);
                time -= 0.1f;
                
                if (time <= 0)
                    break;
                
                if (Configuration.Instance.MakePlayerProne)
                    player.stance.checkStance(EPlayerStance.PRONE, true);
                    
                if (Configuration.Instance.MakePlayerSurrender)
                    player.animator.sendGesture(EPlayerGesture.SURRENDER_START, true);
            }

            if (Configuration.Instance.MakePlayerProne)
                player.stance.checkStance(EPlayerStance.STAND, true);
            
            if (Configuration.Instance.MakePlayerSurrender)
                player.animator.sendGesture(EPlayerGesture.SURRENDER_STOP, true);
            
            if (Configuration.Instance.MakePlayerStop)
            {
                player.movement.sendPluginSpeedMultiplier(1);
                player.movement.sendPluginJumpMultiplier(1);
            }
        }

        private void DamageToolOnDamagePlayerRequested(ref DamagePlayerParameters parameters, ref bool shouldallow)
        {
            shouldallow = true;
            var killer = UnturnedPlayer.FromCSteamID(parameters.killer);
            
            if (killer == null)
                return;
            
            if (!Configuration.Instance.TaserIDs.Contains(killer.Player.equipment.itemID))
                return;
            
            EffectManager.sendEffect(Configuration.Instance.TaserEffectID, 100, parameters.player.transform.position);
            _tasingCoroutines.Add(StartCoroutine(TasingEffects(parameters.player, Configuration.Instance.TasingLength)));
            shouldallow = false;
        }

        protected override void Unload()
        {
            foreach (var tasingCoroutine in _tasingCoroutines)
            {
                StopCoroutine(tasingCoroutine);
            }
            
            DamageTool.damagePlayerRequested -= DamageToolOnDamagePlayerRequested;
            Logger.Log("Kamiluk || Taser plugin has been unloaded.");
        }
    }
}