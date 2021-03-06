﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdvertAPI.Models;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using AutoMapper;

namespace AdvertAPI.Services
{
	public class DynamoDbAdvertStorage : IAdvertStorageService
	{
		private readonly IMapper _mapper;
		public DynamoDbAdvertStorage(IMapper mapper)
		{
			_mapper = mapper;
		}
		public async Task<string> Add(AdvertModel model)
		{
			var dbModel = _mapper.Map<AdvertDbModel>(model);
			dbModel.Id = new Guid().ToString();
			dbModel.CreationDateTime = DateTime.UtcNow;
			dbModel.Status = AdvertStatus.Pending;

			using (var client= new AmazonDynamoDBClient())
			{
				using var context = new DynamoDBContext(client);
				await context.SaveAsync(dbModel);
			}

			return dbModel.Id;
		}

		public async Task Confirm(ConfirmAdvertModel model)
		{
			using (var client = new AmazonDynamoDBClient())
			{
				using (var context = new DynamoDBContext(client))
				{
					var advertisement = await context.LoadAsync<AdvertDbModel>(model.Id);

					if (advertisement == null)
					{
						throw new KeyNotFoundException($"An advertisement with ID={model.Id} was not found");
					}

					if (model.Status == AdvertStatus.Active)
					{
						advertisement.Status = AdvertStatus.Active;
						await context.SaveAsync(advertisement);
					}
					else
					{
						await context.DeleteAsync(advertisement);
					}
				}
			}
		}
	}
}
