local M = {
    name = 'hihi003',
    myaction = -1;
}

function M: startfunction(gamerule)    --��Ϸ�ʼ(��ʼ�����ȴ������Ӧ)  
    print(M['name'] + "��ʼ���ɹ���")
    return M['name']
end

function M: round_start(gamestat)    --ÿ��������ʼ(������ÿ����ҿ���ʱ���)
    print( M['name'] + "���֣�") 
    return
end

function M: action(gamestat)    --ÿ���֡����ö���(������ϸ��Ϣ���ȴ���Ҳ���)
    math.randomseed(tostring(os.time()):reverse():sub(1, 7))
    randNum = math.random(100)
    if (ranNum <= 50) then
        M.myaction = 1
        return M.myaction
    elseif (ranNum <= 90) then
        M.myaction = 2
        return M.myaction
    elseif (ranNum <= 98) then
        M.myaction = 3
        return M.myaction
    end
    M.myaction = 4
    return M.myaction
end

function M: finalCards(gamestat)    --ÿ���֡����ö���(������ϸ��Ϣ���ȴ���Ҳ���)
    math.randomseed(tostring(os.time()):reverse():sub(1, 7))
    first = math.random(10000) % 3
    if (first == 0) then
        second = math.random(10000) % 7
        if (second == 0) then second = second+1 end
    else
        second = 2*first
    end
    list = {}
    no = 0
    for i=0,6,1 do
        if (i == first)  then goto continue end
        if (i == second) then goto continue end
        list[no] = i
        no = no + 1
        ::continue::
    end

    return list
end