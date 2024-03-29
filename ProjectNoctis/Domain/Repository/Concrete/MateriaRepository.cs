﻿using Discord.Commands;
using FuzzySharp;
using ProjectNoctis.Domain.Models;
using ProjectNoctis.Domain.Repository.Interfaces;
using ProjectNoctis.Domain.SheetDatabase;
using ProjectNoctis.Domain.SheetDatabase.Models;
using ProjectNoctis.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectNoctis.Domain.Repository.Concrete
{
    public class MateriaRepository : IMateriaRepository
    {
        private readonly IFfrkSheetContext dbContext;
        private readonly Aliases aliases;

        public MateriaRepository(IFfrkSheetContext dbContext, Aliases aliases)
        {
            this.dbContext = dbContext;
            this.aliases = aliases;
        }

        public List<SheetRecordMaterias> GetRecordMateriasByCharName(string name)
        {
            var charNames = dbContext.Characters.Select(x => x.Name.ToLower());
            var materiaName = dbContext.RecordMaterias.Select(x => x.Name.ToLower()).FirstOrDefault(x => x == name.ToLower());
            var jpName = dbContext.RecordMaterias.FirstOrDefault(x => x.JPName == name);

            if (materiaName == null)
            {
                name = aliases.ResolveAlias(name);

                if (!charNames.Contains(name.ToLower()))
                {
                    name = charNames.OrderByDescending(x => Fuzz.PartialRatio(x, name.ToLower())).FirstOrDefault();
                }
            }

            if (jpName != null)
            {
                return new List<SheetRecordMaterias>() { jpName };
            }

            var recordMaterias = dbContext.RecordMaterias.Where(x => x.Character.ToLower() == name.ToLower() || x.Name.ToLower() == name.ToLower() || x.JPName == name).ToList();         

            return recordMaterias;
        }

        public List<SheetLegendMaterias> GetLegendMateriasByCharName(string name)
        {
            var charNames = dbContext.Characters.Select(x => x.Name.ToLower());
            var materiaName = dbContext.LegendMaterias.Select(x => x.Name.ToLower()).FirstOrDefault(x => x == name.ToLower());
            var jpName = dbContext.LegendMaterias.FirstOrDefault(x => x.JPName == name);

            if (materiaName == null && jpName == null)
            {
                name = aliases.ResolveAlias(name);

                if (!charNames.Contains(name.ToLower()))
                {
                    name = charNames.OrderByDescending(x => Fuzz.PartialRatio(x, name.ToLower())).FirstOrDefault();
                }
            }

            if(jpName != null)
            {
                return new List<SheetLegendMaterias>() { jpName };
            }

            var legendMaterias = dbContext.LegendMaterias.Where(x => x.Character.ToLower() == name.ToLower() || x.Name.ToLower() == name.ToLower() || name == x.JPName).ToList();

            return legendMaterias;
        }

        public List<SheetLegendMaterias> GetLegendMateriasByAnimaWave(string wave)
        {
            var legendMaterias = dbContext.LegendMaterias.Where(x => x.Anima == wave).ToList();

            return legendMaterias;
        }
    }
}
