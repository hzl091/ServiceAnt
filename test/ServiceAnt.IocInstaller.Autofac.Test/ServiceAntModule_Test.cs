﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Autofac;
using ServiceAnt.Handler.Subscription.Handler;
using ServiceAnt.Request.Handler;
using ServiceAnt.Handler;
using ServiceAnt.Subscription.Handler;
using ServiceAnt.Base;
using ServiceAnt.Subscription;
using ServiceAnt.Request;

namespace ServiceAnt.IocInstaller.Autofac.Test
{
    [TestClass]
    public class ServiceAntModule_Test
    {
        private static string RESULT_CONTAINER = "";

        public ServiceAntModule_Test()
        {
        }

        [TestMethod]
        public async Task CanHandleEventByIocHandler()
        {
            var testValue = "HelloWorld";
            var newContainer = new ContainerBuilder();
            newContainer.RegisterModule(new ServiceAntModule(System.Reflection.Assembly.GetExecutingAssembly()));
            var autofacContainer = newContainer.Build();

            await autofacContainer.Resolve<IServiceBus>().Publish(new TestEventTrigger() { Result = testValue });

            Assert.AreEqual(testValue, RESULT_CONTAINER);
        }

        [TestMethod]
        public async Task CanHandleRequestByIocHandler()
        {
            var testValue = "HelloWorld2";
            var newContainer = new ContainerBuilder();
            newContainer.RegisterModule(new ServiceAntModule(System.Reflection.Assembly.GetExecutingAssembly()));
            var autofacContainer = newContainer.Build();

            var result = await autofacContainer.Resolve<IServiceBus>().SendAsync<string>(new TestRequestTrigger() { Result = testValue });

            Assert.AreEqual(testValue, result);
        }

        public class TestEventTrigger : IEventTrigger
        {
            public string Result { get; set; }
        }

        public class TestRequestTrigger : IRequestTrigger
        {
            public string Result { get; set; }
        }

        public class IocEventHandler : IEventHandler<TestEventTrigger>
        {
            public Task HandleAsync(TestEventTrigger param)
            {
                RESULT_CONTAINER = param.Result;

                return Task.Delay(1);
            }
        }

        public class IocRequestHandler : IRequestHandler<TestRequestTrigger>
        {
            public Task HandleAsync(TestRequestTrigger param, IRequestHandlerContext handlerContext)
            {
                handlerContext.Response = param.Result;
                return Task.Delay(1);
            }
        }
    }
}
