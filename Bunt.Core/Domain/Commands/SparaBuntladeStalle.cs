﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bunt.Core.Domain.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Bunt.Core.Domain.Commands
{
    public class SparaBuntladeStalle
    {
        public class Command : IRequest
        {
            public Guid Id { get; set; }
            public string Adress { get; set; }
            public int Typ { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly BuntDbContext _db;
            private readonly ILogger _logger;

            public Handler(BuntDbContext db, ILogger logger)
            {
                _db = db;
                _logger = logger;
            }

            public async Task Handle(Command command, CancellationToken cancellationToken)
            {
                var buntladeStalle = await _db.BuntladeStallen.SingleOrDefaultAsync(b => b.Id == command.Id, cancellationToken);

                if (buntladeStalle == null)
                {
                    var lastIndex = await _db.BuntladeStallen.MaxAsync(b => b.Index as int?, cancellationToken) ?? 0;
                    buntladeStalle = new BuntladeStalle(command.Id, command.Adress, command.Typ, lastIndex + 1);
                    _logger.Information("Nytt buntlådeställe skapat med id: {Id}", buntladeStalle.Id);
                    _db.Add(buntladeStalle);
                }
                else
                {
                    buntladeStalle.Redigera(command.Adress, command.Typ);
                    _logger.Information("Uppdaterade befintligt buntådeställe med id: {Id}", buntladeStalle.Id);
                }

                await _db.SaveChangesAsync(cancellationToken);
            }        
        }
    }
}