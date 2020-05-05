﻿using System.Threading.Tasks;
using AdvertAPI.Models;

namespace AdvertAPI.Services
{
	public interface IAdvertStorageService
	{
		Task<string> Add(AdvertModel model);
		Task<bool> Confirm(ConfirmAdvertModel model);
	}
}

