﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bunt.Core.Infrastructure;
using Dapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bunt.Core.Domain.Commands
{
    public class TaBortBuntladeStalle
    {
        public class Command : IRequest
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly BuntDbContext _db;

            public Handler(BuntDbContext db)
            {
                _db = db;
            }

            public async Task Handle(Command command, CancellationToken cancellationToken)
            {
                var buntladeStalle = await _db.BuntladeStallen.SingleOrDefaultAsync(b => b.Id == command.Id, cancellationToken);

                if (buntladeStalle != null)
                {
                    _db.BuntladeStallen.Remove(buntladeStalle);

                    foreach(var stalle in _db.BuntladeStallen.Where(b => b.Index > buntladeStalle.Index))
                    {
                        stalle.Omsortera(stalle.Index - 1);
                    }
                }

                await _db.SaveChangesAsync(cancellationToken);
            }
        }
    }
}