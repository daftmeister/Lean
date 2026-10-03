# QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
# Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.

from AlgorithmImports import *

### <summary>
### Basic template algorithm demonstrating how to subscribe to Euronext equities and index futures.
### Each Euronext venue is its own Lean market, with its own trading hours, holidays and quote currency.
### </summary>
class BasicTemplateEuronextAlgorithm(QCAlgorithm):

    def initialize(self):
        self.set_start_date(2025, 6, 2)
        self.set_end_date(2025, 6, 27)

        self.set_account_currency(Currencies.EUR)
        self.set_cash(1000000)
        # Euronext Oslo stocks are quoted in Norwegian krone
        self.set_cash(Currencies.NOK, 0)

        self.set_brokerage_model(BrokerageName.INTERACTIVE_BROKERS_BROKERAGE, AccountType.MARGIN)

        self._lvmh = self.add_equity("MC", Resolution.MINUTE, Market.EURONEXT_PARIS)
        self._enel = self.add_equity("ENEL", Resolution.MINUTE, Market.EURONEXT_MILAN)
        self._equinor = self.add_equity("EQNR", Resolution.MINUTE, Market.EURONEXT_OSLO)

        self._cac40 = self.add_future(Futures.Indices.CAC_40, Resolution.MINUTE, Market.EURONEXT_PARIS,
            data_mapping_mode=DataMappingMode.LAST_TRADING_DAY)
        self._cac40.set_filter(timedelta(0), timedelta(60))

        self.set_benchmark(self.add_index("CAC40", Resolution.MINUTE).symbol)

    def on_data(self, slice: Slice):
        if self.portfolio.invested:
            return

        if all(slice.contains_key(x.symbol) for x in [self._lvmh, self._enel, self._equinor]):
            self.set_holdings(self._lvmh.symbol, 0.25)
            self.set_holdings(self._enel.symbol, 0.25)
            self.set_holdings(self._equinor.symbol, 0.25)

        if self._cac40.mapped and slice.contains_key(self._cac40.mapped):
            self.market_order(self._cac40.mapped, 1)
