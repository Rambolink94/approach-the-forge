using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApproachTheForge
{
	public interface Damageable
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="damageData"></param>
		/// <returns> Returns a value indicating whether the target was killed or not. </returns>
		public bool ApplyDamage(DamageData damageData);

	}
}
