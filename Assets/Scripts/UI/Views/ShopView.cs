/**
 * Created Date: 5/9/2023
 * Author: Andrei-Florin Ciobanu
 * 
 * Copyright (c) 2023 Andrei-Florin Ciobanu. All rights reserved. 
 */


using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Managers;
using UnityEngine;

using Shop;
using UI.Buttons;

namespace Views {
	public sealed class ShopView : BaseUIElement {
		[SerializeField]
		private ShopItemButton _shopItemButtonPrefab;

		[SerializeField]
		private Transform _shopItemsParent;
		
		[SerializeField]
		private BaseButton _closeButton;

		[SerializeField]
		private BaseUIElement _notEnoughGoldView;

		private ShopItem[] _items;

		private HashSet<ShopItemButton> _buttons;

		#region Lifecycle

		private void Awake() {
			this._items = Resources.LoadAll<ShopItem>("Scriptables");
			this._buttons = new HashSet<ShopItemButton>();

			foreach (ShopItem shopItem in this._items) {
				ShopItemButton button = GameObject.Instantiate(this._shopItemButtonPrefab, _shopItemsParent);
				this._buttons.Add(button);
				button.Init(shopItem);
				button.SetListener(ShopButtonListener);

				void ShopButtonListener() {
					if (InventoryManager.Instance.CheckIfOwned(shopItem)) {
						button.SetAsOwned();
					} else {
						if (UIManager.Instance.CurrencyPresenter.GoldAmount >= shopItem.Price) {
							UIManager.Instance.CurrencyPresenter.UpdateGoldAmount(-shopItem.Price);	
							InventoryManager.Instance.AddItemToInventory(shopItem);
							button.SetAsOwned();
						} else {
							this.ShowNotEnoughGold();
						}
					}
				}
			}

			this._items = null;
		}

		private void Start() {
			this._closeButton.SetListener(()=> this.Hide(0.2f));
		}

		#endregion
		
		#region Public
		
		public new void Show(float duration, Action completionCallback = null) {
			base.Show(duration, completionCallback);

			foreach (ShopItemButton shopItemButton in this._buttons) {
				if (InventoryManager.Instance.CheckIfOwned((shopItemButton.Item))) {
					shopItemButton.SetAsOwned();
				} else {
					shopItemButton.SetPrice();
				}
			}
		}
		
		#endregion
		
		#region Private
		
		/// <summary>
		/// Show not enough gold popup.
		/// </summary>
		private async void ShowNotEnoughGold() {
			this._notEnoughGoldView.Show(0.2f);
			await Task.Delay(500);
			this._notEnoughGoldView.Hide(0.2f);
		}
		
		#endregion
	}
}