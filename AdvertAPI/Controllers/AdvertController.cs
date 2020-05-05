﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdvertAPI.Models;
using AdvertAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AdvertAPI.Controllers
{
	[ApiController]
	[Route("[api/v1/adverts]")]
	public class AdvertController : ControllerBase
	{
		private readonly IAdvertStorageService _advertStorageService;

		public AdvertController(IAdvertStorageService advertStorageService)
		{
			_advertStorageService = advertStorageService;
		}

		[HttpPost]
		[Route("Create")]
		[ProducesResponseType(400)]
		[ProducesResponseType(200, Type = typeof(CreateAdvertResponse))]

		public async Task<IActionResult> Create(AdvertModel model)
		{
			string recordId;
			try
			{
				recordId = await _advertStorageService.Add(model);
			}
			catch (KeyNotFoundException)
			{
				return new NotFoundResult();
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex.Message);
			}

			return StatusCode(201, new CreateAdvertResponse { Id = recordId });
		}

		[HttpPut]
		[Route("Confirm")]
		[ProducesResponseType(404)]
		[ProducesResponseType(200)]

		public async Task<IActionResult> Confirm(ConfirmAdvertModel model)
		{
			try
			{
				await _advertStorageService.Confirm(model);
			}
			catch (KeyNotFoundException)
			{
				return new NotFoundResult();
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex.Message);
			}

			return new OkResult();
		}
	}
}
