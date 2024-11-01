﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Application.Configuration.Queries
{
    public interface IQuery<out TResult> :IRequest<TResult>
    {
    }
}
