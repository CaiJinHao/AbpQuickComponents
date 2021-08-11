﻿using Jh.Abp.Application.Contracts.Extensions;

using Jh.Abp.MenuManagement.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Data;
using System.Linq;
using Volo.Abp.PermissionManagement;

namespace Jh.Abp.MenuManagement.v1
{
    [RemoteService]
    [Route("api/v{apiVersion:apiVersion}/[controller]")]
    public class MenuController: MenuManagementController
    {
        public IPermissionDefinitionManager PermissionDefinitionManager { get; set; }
        public IPermissionAppService permissionAppService { get; set; }
        private readonly IMenuAppService menuAppService;
        public IDataFilter<ISoftDelete> dataFilter { get; set; }

        public MenuController(IMenuAppService _menuAppService)
        {
            menuAppService = _menuAppService;
        }

        /// <summary>
        /// 创建一个实体
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
		[Authorize(MenuManagementPermissions.Menus.Create)]
        [HttpPost]
        public virtual async Task CreateAsync(MenuCreateInputDto input)
        {
            await menuAppService.CreateAsync(input,true);
        }

        /// <summary>
        /// 创建多个实体
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
		[Authorize(MenuManagementPermissions.Menus.BatchCreate)]
        [Route("items")]
        [HttpPost]
        public virtual async Task CreateAsync(MenuCreateInputDto[] input)
        {
            await menuAppService.CreateAsync(input);
        }

        /// <summary>
        /// 根据条件删除多条
        /// </summary>
        /// <param name="deleteInputDto"></param>
        /// <returns></returns>
		[Authorize(MenuManagementPermissions.Menus.BatchDelete)]
        [HttpDelete]
        public virtual async Task DeleteAsync([FromQuery] MenuDeleteInputDto deleteInputDto)
        {
            await menuAppService.DeleteAsync(deleteInputDto);
        }

        /// <summary>
        /// 根据主键删除多条
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
		[Authorize(MenuManagementPermissions.Menus.BatchDelete)]
        [Route("keys")]
        [HttpDelete]
        public virtual async Task DeleteAsync([FromBody]Guid[] keys)
        {
            await menuAppService.DeleteAsync(keys);
        }

        /// <summary>
        /// 根据条件查询(不分页)
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
		[Authorize(MenuManagementPermissions.Menus.Export)]
        [Route("all")]
        [HttpGet]
        public virtual async Task<ListResultDto<MenuDto>> GetEntitysAsync([FromQuery] MenuRetrieveInputDto inputDto)
        {
            return await menuAppService.GetEntitysAsync(inputDto);
        }

        /// <summary>
        /// 根据id更新部分
        /// </summary>
        /// <param name="key"></param>
        /// <param name="inputDto"></param>
        /// <returns></returns>
		[Authorize(MenuManagementPermissions.Menus.PortionUpdate)]
        [HttpPatch("{id}")]
        public virtual async Task UpdatePortionAsync(Guid id, MenuUpdateInputDto inputDto)
        {
            await menuAppService.UpdatePortionAsync(id, inputDto);
        }

        /// <summary>
        /// 根据ID更新全部
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
		[Authorize(MenuManagementPermissions.Menus.Update)]
        [HttpPut("{id}")]
        public virtual async Task<MenuDto> UpdateAsync(Guid id, MenuUpdateInputDto input)
        {
            return await menuAppService.UpdateAsync(id, input);
        }

        /// <summary>
        /// 根据ID删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		[Authorize(MenuManagementPermissions.Menus.Delete)]
        [HttpDelete("{id}")]
        public virtual async Task DeleteAsync(Guid id)
        {
            using (dataFilter.Disable())
            {
                await menuAppService.DeleteAsync(id, autoSave: true, isHard: false);
            }
        }

        /// <summary>
        /// 根据ID获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		[Authorize(MenuManagementPermissions.Menus.Detail)]
        [HttpGet("{id}")]
        public virtual async Task<MenuDto> GetAsync(Guid id)
        {
            return await menuAppService.GetAsync(id, true);
        }

		[Authorize(MenuManagementPermissions.Menus.Recover)]
        [HttpPatch]
        [Route("{id}/Deleted")]
        public virtual async Task UpdateDeletedAsync(Guid id, [FromBody] bool isDeleted)
        {
            using (dataFilter.Disable())
            {
                await menuAppService.UpdatePortionAsync(id, new MenuUpdateInputDto()
                {
                    MethodInput = new MethodDto<Menu>()
                    {
                        CreateOrUpdateEntityAction = (entity) => entity.IsDeleted = isDeleted
                    }
                });
            }
        }

        /// <summary>
        /// 根据条件分页查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(MenuManagementPermissions.Menus.Default)]
        [HttpGet]
        public virtual async Task<PagedResultDto<MenuDto>> GetListAsync([FromQuery] MenuRetrieveInputDto input)
        {
            using (dataFilter.Disable())
            {
                if (!string.IsNullOrEmpty(input.OrCode))
                {
                    input.MethodInput = new MethodDto<Menu>()
                    {
                        QueryAction = entity => entity.Where(a => a.ParentCode == input.OrCode || a.Code == input.OrCode)
                    };
                }
                return await menuAppService.GetListAsync(input);
            }
        }
    }
}
