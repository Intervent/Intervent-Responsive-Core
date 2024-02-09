function Dirctionry() {
    var items = [];

    this.Add = function (key, value) {
        var item = new Item(key, value);
        items.push(item);
    }

    this.Update = function (key, value) {
        var item = this.Find(key);
        if (item == null) {
            this.Add(key, value);
        }
        else {
            item.Value = item.Value + value;
        }
    }

    this.Count = function () {
        return items.length;
    }

    this.Get = function (index) {
        return items[index];
    }

    this.Find = function (key) {
        for (var i = 0; i < items.length; i++) {
            var item = items[i];
            if (item.Key == key) {
                return item;
            }
        }
        return null;
    }
}

function Item(key, value) {
    this.Key = key;
    this.Value = value;
}