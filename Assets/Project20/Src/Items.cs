using System;
using System.Collections.Generic;
using Blaze.Runtime.Cms;

namespace Proj21
{
    public class ItemStack
    {
        public CmsEntity item;
        public int count;
        public int maxCount = int.MaxValue;

        public ItemStack(CmsEntity item, int count, int maxCount = int.MaxValue)
        {
            this.item = item;
            this.count = count;
            this.maxCount = maxCount;
        }

        public void Remove(int count)
        {
            this.count = Math.Clamp(this.count - count, 0, maxCount);
        }
    }

    public class ItemsSystem
    {
        public Dictionary<CmsEntity, ItemStack> stacks = new();

        public void Reset()
        {
            stacks = new();
        }

        public ItemStack GetStack(CmsEntity item)
        {
            ItemStack stack;
            if (!stacks.TryGetValue(item, out stack))
            {
                stack = new ItemStack(item, 0);
                stacks[item] = stack;
            }
            return stack;
        }

        public void Add(ItemStack stack)
        {
            GetStack(stack.item).count += stack.count;
        }
        public void Remove(ItemStack stack)
        {
            GetStack(stack.item).Remove(stack.count);
        }
        public bool Has(ItemStack stack)
        {
            return GetStack(stack.item).count >= stack.count;
        }
    }

    public static class Items
    {
        public static CmsEntity Essence => Cms.GetEntity("Essence");

        public static List<CmsEntity> All => new() {Essence};
    }
}