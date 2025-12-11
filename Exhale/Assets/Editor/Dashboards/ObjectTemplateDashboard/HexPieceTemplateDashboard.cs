#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Exhale.Scripts.Data;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;
using Toggle = UnityEngine.UIElements.Toggle;

namespace Exhale.Scripts.Editor.Dashboards
{
	public class HexPieceTemplateDashboard : EditorWindow
	{
		public VisualTreeAsset RootTreeTemplate;
		public VisualTreeAsset ObjectRowTemplate;

		private Dictionary<string, HexPieceTemplate> objectTemplatePathsToConfig = new();
		private Dictionary<Type, Toggle> traitToggleFilters = new();
		private Dictionary<Type, string> traitToggleFiltersIds = new();
		private string objectNameQuery = "";
		private int numErrors = 0;

		[MenuItem("Exhale/🕵 Dashboards/HexPiece Templates")]
		public static void ShowWindow()
		{
			HexPieceTemplateDashboard wnd = GetWindow<HexPieceTemplateDashboard>();
			wnd.titleContent = new GUIContent("[Dashboard] HexPiece Templates");
		}

		public void CreateGUI()
		{
			VisualElement root = rootVisualElement;
			root.Add(RootTreeTemplate.CloneTree());

			LoadObjectTemplates();
			SetupQuickActions();
			SetupFilters();
			RefreshList();
		}

		private void LoadObjectTemplates()
		{
			objectTemplatePathsToConfig.Clear();

			foreach(HexPieceTemplate template in HexPieceTemplateCollection.Values)
			{
				string configPath = AssetDatabase.GetAssetPath(template);
				objectTemplatePathsToConfig.Add(configPath, template);
			}
		}

		private void Reload()
		{
			LoadObjectTemplates();
			RefreshList();
		}
		
		private void SetupQuickActions()
		{
			ToolbarMenu toolbarMenuNew = rootVisualElement.Q<ToolbarMenu>("toolbar-menu-new-object");
			Assert.IsNotNull(toolbarMenuNew, "Visual element toolbar-menu-new-object not found");

			/*toolbarMenuNew.menu.AppendAction("Customization/Human item", _ => CreateNewObjectTemplate(DefaultObjectTemplateConstructors.CreateAssetItemRiderCustomization));
			toolbarMenuNew.menu.AppendAction("Customization/Unicorn item", _ => CreateNewObjectTemplate(DefaultObjectTemplateConstructors.CreateAssetItemUnicornCustomization));
			toolbarMenuNew.menu.AppendAction("Emote/Body", _ => CreateNewObjectTemplate(DefaultObjectTemplateConstructors.CreateAssetBodyEmote));
			toolbarMenuNew.menu.AppendAction("Emote/Speech", _ => CreateNewObjectTemplate(DefaultObjectTemplateConstructors.CreateAssetSpeechEmote));
			toolbarMenuNew.menu.AppendAction("Shop/Buyable item", _ => CreateNewObjectTemplate(DefaultObjectTemplateConstructors.CreateAssetShopBuyableItem));
			toolbarMenuNew.menu.AppendAction("Shop/Saleable item", _ => CreateNewObjectTemplate(DefaultObjectTemplateConstructors.CreateAssetShopSaleableItem));
			toolbarMenuNew.menu.AppendAction("Shop/Buyable + Saleable item", _ => CreateNewObjectTemplate(DefaultObjectTemplateConstructors.CreateAssetShopBuyableSaleableItem));
			toolbarMenuNew.menu.AppendAction("Default", _ => CreateNewObjectTemplate(DefaultObjectTemplateConstructors.CreateAssetDefaultItem));*/
		}

		private void CreateNewObjectTemplate(Action objectTemplateConstructor)
		{
			objectTemplateConstructor();
			Reload();
		}

		private void SetupFilters()
		{
			ToolbarSearchField toolbarSearchField = rootVisualElement.Q("toolbar-search-object-name") as ToolbarSearchField;
			Assert.IsNotNull(toolbarSearchField, "Visual element toolbar-search-object-name not found");
			toolbarSearchField.RegisterCallback<ChangeEvent<string>>(evt =>
			{
				objectNameQuery = evt.newValue;
				RefreshList();
			});
			
			
			/*traitToggleFiltersIds.Add(typeof(Emote), "toggle-emote-trait");
			traitToggleFiltersIds.Add(typeof(RiderCustomizationTrait), "toggle-rider-customization-items-trait");
			traitToggleFiltersIds.Add(typeof(UnicornCustomizationTrait), "toggle-unicorn-customization-items-trait");
			
			traitToggleFiltersIds.Add(typeof(Rarity), "toggle-rarity-trait");
			traitToggleFiltersIds.Add(typeof(Indexing), "toggle-indexing-trait");
			
			traitToggleFiltersIds.Add(typeof(Networkable), "toggle-networkable-trait");
			traitToggleFiltersIds.Add(typeof(Interaction), "toggle-interaction-trait");
			traitToggleFiltersIds.Add(typeof(Consumable), "toggle-consumable-trait");
			traitToggleFiltersIds.Add(typeof(Giftable), "toggle-giftable-trait");
			traitToggleFiltersIds.Add(typeof(WorldObject), "toggle-world-object-trait");
			traitToggleFiltersIds.Add(typeof(Thumbnail), "toggle-thumbnail-trait");
			traitToggleFiltersIds.Add(typeof(Buyable), "toggle-buyable-trait");
			traitToggleFiltersIds.Add(typeof(Saleable), "toggle-saleable-trait");
			traitToggleFiltersIds.Add(typeof(Bindable), "toggle-bindable-trait");
			traitToggleFiltersIds.Add(typeof(Inventory), "toggle-inventory-trait");
			traitToggleFiltersIds.Add(typeof(Feedable), "toggle-feedable-trait");
			traitToggleFiltersIds.Add(typeof(BondConstellation), "toggle-bond-constellation-trait");
			
			traitToggleFiltersIds.Add(typeof(Npc), "toggle-npc-trait");
			traitToggleFiltersIds.Add(typeof(Talker), "toggle-talker-trait");
			traitToggleFiltersIds.Add(typeof(Buyer), "toggle-buyer-trait");
			traitToggleFiltersIds.Add(typeof(Seller), "toggle-seller-trait");
			traitToggleFiltersIds.Add(typeof(Friendable), "toggle-friendable-trait");
			
			foreach (var toggleFiltersId in traitToggleFiltersIds)
			{
				Toggle toggleTraitElement = rootVisualElement.Q<Toggle>(toggleFiltersId.Value);
				Assert.IsNotNull(toggleTraitElement, $"Visual element {toggleFiltersId.Value} not found");
				toggleTraitElement.RegisterCallback(delegate(ChangeEvent<bool> evt)
				{
					traitToggleFilters[toggleFiltersId.Key].value = evt.newValue;
					RefreshList();
				});
				traitToggleFilters.Add(toggleFiltersId.Key, toggleTraitElement);
			}

			SetupIndexingSubFilters();
			SetupRaritySubFilters();
			SetupCustomizationSubFilters();
			SetupEmotesSubFilters();*/
		}

		/*private void SetupRaritySubFilters()
		{
			VisualElement containerRarityTraitFilterDetails = rootVisualElement.Q<VisualElement>("container-rarity-filter-details");
			Assert.IsNotNull(containerRarityTraitFilterDetails, $"Visual element container-rarity-filter-details not found");
			Toggle toggleRarityTrait = rootVisualElement.Q<Toggle>("toggle-rarity-trait");
			Assert.IsNotNull(toggleRarityTrait, $"Visual element toggle-rarity-trait not found");
			toggleRarityTrait.RegisterCallback(delegate(ChangeEvent<bool> evt) { containerRarityTraitFilterDetails.SetEnabled(evt.newValue); });
			containerRarityTraitFilterDetails.SetEnabled(false);
			
			EnumField rarityFilter = rootVisualElement.Q<EnumField>("rarity-filter");
			Assert.IsNotNull(rarityFilter, "Visual element rarity-filter not found");
			rarityFilter.Init(RarityType.Common);
			rarityFilter.RegisterCallback<ChangeEvent<Enum>>((evt) => { RefreshList(); });
			rarityFilter.RegisterCallback(delegate(ChangeEvent<bool> evt) { containerRarityTraitFilterDetails.SetEnabled(evt.newValue); });
			containerRarityTraitFilterDetails.SetEnabled(false);
		}*/
		
		private static List<string> GetSelectedChoices(List<string> choices, int maskFieldValue)
		{
			List<string> selectedChoices = new();

			for (int i = 0; i < choices.Count; i++)
			{
				int layer = 1 << i;
				if ((maskFieldValue & layer) != 0)
				{
					selectedChoices.Add(choices[i]);
				}
			}

			return selectedChoices;
		}

		private void RefreshList()
		{
			ScrollView scrollViewObjectTemplates = rootVisualElement.Q("scrollview-object-templates") as ScrollView;
			Assert.IsNotNull(scrollViewObjectTemplates, $"Visual element scrollview-object-templates not found");

			Label labelNumObjects = rootVisualElement.Q<Label>("label-num-objects");        
			Assert.IsNotNull(labelNumObjects, "Visual element label-num-objects not found");
			
			Label labelNumErrors = rootVisualElement.Q<Label>("label-num-errors");                      
			Assert.IsNotNull(labelNumErrors, "Visual element label-num-errors not found");              
			
			numErrors = 0;
			scrollViewObjectTemplates.Clear();
			if (!IsAnyToggleSelected())
			{
				foreach (KeyValuePair<string, HexPieceTemplate> objectTemplate in objectTemplatePathsToConfig.OrderBy(x => x.Value.name))
				{
					TemplateContainer objectLineItem = ProcessGameObjectsRow(objectTemplate.Value);
					string objectTemplateName = objectTemplate.Value.name.ToLower();
					int templateId = objectTemplate.Value.GetId();
					if (objectTemplateName.Contains(objectNameQuery.ToLower()) || objectNameQuery.Contains(templateId.ToString("X8")))
					{
						scrollViewObjectTemplates.Add(objectLineItem);
					}
				}
			}
			else
			{
				IEnumerable<HexPieceTemplate> filteredObjects = GetFilteredObjects();
				foreach (TemplateContainer objectLineItem in filteredObjects.Select(ProcessGameObjectsRow))
				{
					scrollViewObjectTemplates.Add(objectLineItem);
				}
			}
			
			labelNumObjects.text = scrollViewObjectTemplates.childCount == objectTemplatePathsToConfig.Count ? 
				$"Total objects: <b>{objectTemplatePathsToConfig.Count}</b>" : 
				$"Total objects: <b>{scrollViewObjectTemplates.childCount}/{objectTemplatePathsToConfig.Count}</b>";
			
			labelNumErrors.text = $"Errors: {numErrors}";
		}

		private IEnumerable<HexPieceTemplate> GetFilteredObjects()
		{
			List<HexPieceTemplate> filteredObjects = new();

			foreach (HexPieceTemplate objectTemplate in HexPieceTemplateCollection.Values)
			{
				if (objectTemplate == null) continue;

				foreach (PieceTrait trait in from traitToggleFilter in traitToggleFilters
				         where traitToggleFilter.Value.value
				         from trait in objectTemplate.Traits
				         where trait != null && trait.GetType() == traitToggleFilter.Key
				         select trait)
				{
					filteredObjects.Add(objectTemplate);
				}
			}

			return filteredObjects.Where(x => x.name.ToLower().Contains(objectNameQuery.ToLower())).Distinct();
		}

		private bool IsAnyToggleSelected()
		{
			return traitToggleFilters.Any(traitToggleFilter => traitToggleFilter.Value.value);
		}

		private TemplateContainer ProcessGameObjectsRow(HexPieceTemplate objectTemplate)
		{
			TemplateContainer objectLineItem = ObjectRowTemplate.Instantiate();
			Label labelObjectName = objectLineItem.Q("object-name") as Label;
			Assert.IsNotNull(labelObjectName, $"Visual element object-name not found");
			labelObjectName.text = objectTemplate.name;
			
			if (objectTemplate.TryGetTrait(out Thumbnail thumbnail))
			{
				VisualElement objectThumbnail = objectLineItem.Q("object-thumbnail-image");
				Assert.IsNotNull(objectThumbnail, $"Visual element object-thumbnail-image not found");
				objectThumbnail.style.backgroundImage = new StyleBackground(thumbnail.Sprite);
			}

			VisualElement containerObjectTraits = objectLineItem.Q("object-traits");
			Assert.IsNotNull(containerObjectTraits, $"Visual element object-traits not found");
			containerObjectTraits.Clear();
			foreach (PieceTrait templateTrait in objectTemplate.Traits.Where(x => x != null).OrderBy(x => x.GetType().Name))
			{
				Label labelObjectTrait = new() {
					text = templateTrait.GetType().Name
				};
				
				// check if there is any error
				labelObjectTrait.AddToClassList(!templateTrait.ValidateConfig(objectTemplate) ? "list-label-object-template-error" : "list-label-object-template");
				if (!templateTrait.ValidateConfig(objectTemplate))
				{
					labelObjectTrait.AddToClassList("list-label-object-template-error");
					numErrors++;
				}
				else
				{
					labelObjectTrait.AddToClassList("list-label-object-template");
				}

				traitToggleFilters.TryGetValue(templateTrait.GetType(), out Toggle toggleTrait);
				if (toggleTrait is not null && toggleTrait.value)
				{
					labelObjectTrait.AddToClassList("label-highlight");
				}

				containerObjectTraits.Add(labelObjectTrait);
			}

			Button buttonOpenConfig = objectLineItem.Q("button-select") as Button;
			Assert.IsNotNull(buttonOpenConfig, $"Visual element button-select not found");
			buttonOpenConfig.clickable.clicked += () =>
			{
				Selection.activeObject = objectTemplate;
			};

			return objectLineItem;
		}
	}
}
#endif