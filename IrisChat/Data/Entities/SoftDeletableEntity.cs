﻿    namespace IrisChat.Data.Entities
    {
        public abstract class SoftDeletableEntity
        {
            public bool IsDeleted { get; set; }
            public DateTime? DeletedAt { get; set; }
        }
    }
