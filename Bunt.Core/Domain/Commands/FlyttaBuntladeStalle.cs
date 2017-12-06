using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bunt.Core.Domain.Commands
{
    public class FlyttaBuntladeStalle
    {
        public class Command : IRequest
        {
            public int NewIndex { get; set; }
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
                var buntladeStalle = await _db.BuntladeStallen.SingleOrDefaultAsync(c => c.Id == command.Id, cancellationToken);

                if (buntladeStalle != null)
                {
                    var stalleToSwitchWith = await _db.BuntladeStallen.SingleOrDefaultAsync(b => b.Index == command.NewIndex, cancellationToken);

                    if (stalleToSwitchWith != null)
                    { 
                        stalleToSwitchWith.Omsortera(buntladeStalle.Index);
                        buntladeStalle.Omsortera(command.NewIndex);
                    }
                }

                await _db.SaveChangesAsync(cancellationToken);
            }
        }
    }
}