import {
  BrowserRouter,
  Routes,
  Route,
} from "react-router-dom";
import Login from "./pages/Login";
import Kitchens from "./pages/Kitchens";
import Kitchen from "./pages/Kitchen";
import KitchenAdd from "./pages/KitchenAdd";
import KitchenUpdate from "./pages/KitchenUpdate";

function App() {
  return (
    <div className="App">
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<Login/>}/>
          <Route path="/kitchens" element={<Kitchens/>}/>
          <Route path="/kitchen/:id" element={<Kitchen/>}/>
          <Route path="/kitchenadd" element={<KitchenAdd/>}/>
          <Route path="/kitchenupdate/:id" element={<KitchenUpdate/>}/>
        </Routes>
      </BrowserRouter>
    </div>
  );
}

export default App;
