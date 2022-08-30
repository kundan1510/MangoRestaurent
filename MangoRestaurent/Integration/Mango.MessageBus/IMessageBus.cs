﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mango.MessageBus
{
    internal interface IMessageBus
    {
        Task PublishMessage(BaseMessage message, string topicName);
    }
}
