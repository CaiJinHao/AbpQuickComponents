﻿using Jh.Abp.Extensions;
using System;

namespace Jh.Abp.MenuManagement
{
    public interface IMenuAppService
         : ICrudApplicationService<Menu, MenuDto, MenuDto, Guid, MenuRetrieveInputDto, MenuCreateInputDto, MenuUpdateInputDto, MenuDeleteInputDto>
    {

    }
}
