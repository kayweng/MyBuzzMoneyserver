﻿using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using MyBuzzMoney.Library.Models;
using MyBuzzMoney.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AWSSimpleClients.Clients;
using MyBuzzMoney.Library.Helpers;
using MyBuzzMoney.Library.Enums;
using Newtonsoft.Json;

namespace MyBuzzMoney.Repository
{
    public class SettingRepository : BaseRepository<UserSetting>, ISettingRepository
    {
        public SettingRepository(string tableName) : base(tableName)
        {
           
        }

        public async Task<UserSetting> RetrieveUserSetting(string username)
        {
            try
            {
                var userSetting = await DDBContext.LoadAsync<UserSetting>(username);
               
                return userSetting;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> UpdatePreferences(UserSetting userSetting)
        {
            try
            {
                var request = new UpdateItemRequest
                {
                    TableName = DynamoTableName,
                    Key = new Dictionary<string, AttributeValue>()
                    {
                        { "Email", new AttributeValue { S = userSetting.Email } }
                    },
                    ExpressionAttributeNames = new Dictionary<string, string>()
                    {
                        {"#Preferences", "Preferences"},
                        {"#ModifiedOn", "ModifiedOn"}
                    },
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
                    {
                        {":preferences", new AttributeValue { S = userSetting.Preferences.Replace("\\\\","") }},
                        {":modifiedOn", new AttributeValue { S = userSetting.ModifiedOn }}
                    },

                    UpdateExpression = "SET #Preferences = :preferences, #ModifiedOn = :modifiedOn"
                };

                return await AWS.DynamoDB.UpdateItemAsync(request).ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        Console.WriteLine(task.Exception.Message);
                    }

                    return !task.IsFaulted;
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
