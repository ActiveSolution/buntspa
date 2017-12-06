﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bunt.Core.Domain.Commands
{
    public class FlyttaUppBuntladeStalle
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

                if (buntladeStalle == null)
                    return;

                var nextBuntladeStalle = await _db.BuntladeStallen.SingleOrDefaultAsync(b => b.Index == buntladeStalle.Index + 1, cancellationToken);

                if (nextBuntladeStalle == null)
                    return;
                
                buntladeStalle.Omsortera(buntladeStalle.Index + 1);
                nextBuntladeStalle.Omsortera(buntladeStalle.Index - 1);

                await _db.SaveChangesAsync(cancellationToken);
            }
        }
    }
}