﻿namespace Reporte.Recolector.Dtos
{
    public class TransactionDTO
    {
        public Guid Id { get; set; }

        public List<string> errors { get; set; }
    }
}
