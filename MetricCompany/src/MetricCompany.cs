using BepInEx;
using HarmonyLib;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

namespace MetricCompany
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInProcess("Lethal Company.exe")]
    public class MetricCompany : BaseUnityPlugin
    {
        private Harmony harmony;

        [HarmonyPatch]
        private class HUDDrawHook
        {
            [HarmonyPatch(typeof(HUDManager), "Update")]
            [HarmonyPostfix]
            private static void ConvertLbsToKg(ref TextMeshProUGUI ___weightCounter, ref Animator ___weightCounterAnimator)
            {
                // Perform calculation that the game uses to determine weight in pounds, then convert to kilograms
                float weight_in_lbs = Mathf.Clamp(GameNetworkManager.Instance.localPlayerController.carryWeight - 1f, 0f, 100f) * 105f;
                float weight_in_kgs = weight_in_lbs / 2.205f;

                // Assign the text label and set the animator's weight property
                ___weightCounter.text = string.Format("{0:0.#}", weight_in_kgs) + " kg";
                ___weightCounterAnimator.SetFloat("weight", weight_in_kgs / 130f);

                return;
            }
        }

        private void Awake()
        {
            // Create a Harmony instance and enable all patches
            harmony = new Harmony($"{PluginInfo.PLUGIN_GUID}");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} is loaded");

            return;
        }
    }
}