using System;

namespace AutoMapper
{
	internal class ResolutionExpression<TSource> : IResolverConfigurationExpression<TSource>, IResolverConfigurationExpression
	{
	    private readonly Type _sourceType;
	    private readonly PropertyMap _propertyMap;

        public ResolutionExpression(PropertyMap propertyMap) : this(typeof(TSource), propertyMap) {}

	    public ResolutionExpression(Type sourceType, PropertyMap propertyMap)
	    {
	        _sourceType = sourceType;
	        _propertyMap = propertyMap;
	    }

	    public void FromMember(Func<TSource, object> sourceMember)
		{
			_propertyMap.ChainTypeMemberForResolver(new DelegateBasedResolver<TSource>(sourceMember));
		}

		public void FromMember(string sourcePropertyName)
		{
			_propertyMap.ChainTypeMemberForResolver(new PropertyNameResolver(_sourceType, sourcePropertyName));
		}

	    IResolutionExpression IResolverConfigurationExpression.ConstructedBy(Func<IValueResolver> constructor)
	    {
	        return ConstructedBy(constructor);
	    }

	    public IResolutionExpression<TSource> ConstructedBy(Func<IValueResolver> constructor)
		{
			_propertyMap.ChainConstructorForResolver(new DeferredInstantiatedResolver(constructor));

			return this;
		}
	}

    internal class ResolutionExpression : ResolutionExpression<object>
    {
        public ResolutionExpression(Type sourceType, PropertyMap propertyMap) : base(sourceType, propertyMap) {}
    }

	internal class ResolutionExpression<TSource, TValueResolver> : IResolverConfigurationExpression<TSource, TValueResolver>
		where TValueResolver : IValueResolver
	{
		private readonly PropertyMap _propertyMap;

		public ResolutionExpression(PropertyMap propertyMap)
		{
			_propertyMap = propertyMap;
		}

		public IResolverConfigurationExpression<TSource, TValueResolver> FromMember(Func<TSource, object> sourceMember)
		{
			_propertyMap.ChainTypeMemberForResolver(new DelegateBasedResolver<TSource>(sourceMember));

			return this;
		}

		public IResolverConfigurationExpression<TSource, TValueResolver> FromMember(string sourcePropertyName)
		{
			_propertyMap.ChainTypeMemberForResolver(new PropertyNameResolver(typeof(TSource), sourcePropertyName));

			return this;
		}

		public IResolverConfigurationExpression<TSource, TValueResolver> ConstructedBy(Func<TValueResolver> constructor)
		{
			_propertyMap.ChainConstructorForResolver(new DeferredInstantiatedResolver(() => constructor()));

			return this;
		}
	}
}