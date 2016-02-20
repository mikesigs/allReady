﻿using AllReady.Features.Organizations;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Features.Organizations
{
    public class OrganizationPrivacyPolicyQueryHandlerAsyncTests : InMemoryContextTest
    {
        [Fact]
        public async Task OrgWithNoPrivacyPolicy_ReturnsNullContent()
        {
            var handler = new OrganizationPrivacyPolicyQueryHandlerAsync(Context);
            var result = await handler.Handle(new OrganziationPrivacyPolicyQueryAsync { Id = 1 });

            Assert.NotNull(result);
            Assert.Equal("Org 1", result.OrganizationName);
            Assert.Null(result.Content);
        }

        [Fact]
        public async Task OrgWithValidPrivacyPolicy_ReturnsContent()
        {
            var handler = new OrganizationPrivacyPolicyQueryHandlerAsync(Context);
            var result = await handler.Handle(new OrganziationPrivacyPolicyQueryAsync { Id = 2 });

            Assert.NotNull(result);
            Assert.Equal("Org 2", result.OrganizationName);
            Assert.Equal("Line 1<br />Line 2",result.Content);
        }

        [Fact]
        public async Task OrgWithHtmlPrivacyPolicy_ReturnsContentWithStrippedHtml()
        {
            var handler = new OrganizationPrivacyPolicyQueryHandlerAsync(Context);
            var result = await handler.Handle(new OrganziationPrivacyPolicyQueryAsync { Id = 3 });

            Assert.NotNull(result);
            Assert.Equal("Org 3", result.OrganizationName);
            Assert.Equal("Line 1<br />Line 2", result.Content);
        }

        [Fact]
        public async Task NullReturnedWhenSkillIdDoesNotExists()
        {
            var handler = new OrganizationPrivacyPolicyQueryHandlerAsync(Context);
            var result = await handler.Handle(new OrganziationPrivacyPolicyQueryAsync { Id = 100 });

            Assert.Null(result);
        }

        [Fact]
        public async Task NullReturnedWhenSkillIdNotInMessage()
        {
            var handler = new OrganizationPrivacyPolicyQueryHandlerAsync(Context);
            var result = await handler.Handle(new OrganziationPrivacyPolicyQueryAsync());

            Assert.Null(result);
        }

        protected override void LoadTestData()
        {
            OrganizationHandlerTestHelper.LoadOrganizationHandlerTestData(Context);
        }
    }
}